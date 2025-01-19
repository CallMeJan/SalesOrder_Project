using Project_SalesOrder.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project_SalesOrder.Controllers
{
    public class SalesOrderController : Controller
    {

        private readonly MyDatabaseContext _context;
        public SalesOrderController()
        {
            _context = new MyDatabaseContext();
        }

        public ActionResult Index(string keywords, DateTime? orderDate)
        {
            var query = _context.Orders.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(keywords))
            {
                query = query.Where(o => o.ORDER_NO.Contains(keywords) ||
                                         o.Customer.CUSTOMER_NAME.Contains(keywords));
            }

            if (orderDate.HasValue)
            {
                var startDate = orderDate.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(o => o.ORDER_DATE >= startDate && o.ORDER_DATE < endDate);
            }

            var orders = query.ToList();

            ViewBag.Keywords = keywords;
            ViewBag.OrderDate = orderDate?.ToString("yyyy-MM-dd");
            return View(orders);
        }

        public ActionResult Create()
        {
            var model = new Order
            {
                ORDER_DATE = DateTime.Now 
            };
            ViewBag.Customers = _context.Customers.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Order order)
        {
            bool isOrderNoExists = _context.Orders.Any(o => o.ORDER_NO == order.ORDER_NO);
            if (isOrderNoExists)
            {
                ModelState.AddModelError("ORDER_NO", "Sales Order Number already exists.");
                ViewBag.Customers = _context.Customers.ToList();
                return View(order);
            }

            if (string.IsNullOrWhiteSpace(order.ADDRESS))
            {
                order.ADDRESS = "";
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var newOrder = new Order
                    {
                        ORDER_NO = order.ORDER_NO,
                        COM_CUSTOMER_ID = order.COM_CUSTOMER_ID,
                        ORDER_DATE = order.ORDER_DATE,
                        ADDRESS = order.ADDRESS
                    };

                    _context.Orders.Add(newOrder);
                    _context.SaveChanges();

                    var orderId = newOrder.SO_ORDER_ID;

                    if (order.Items != null && order.Items.Any())
                    {
                        foreach (var item in order.Items)
                        {
                            var newItem = new Item
                            {
                                ITEM_NAME = item.ITEM_NAME,
                                QUANTITY = item.QUANTITY,
                                PRICE = item.PRICE,
                                SO_ORDER_ID = orderId 
                            };

                            _context.Items.Add(newItem);
                        }
                    }

                    _context.SaveChanges();

                    transaction.Commit();

                    TempData["Success"] = true;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Error: " + ex.Message);
                    ViewBag.Customers = _context.Customers.ToList();
                    return View(order);
                }
            }
        }

        public ActionResult Edit(long id)
        {
            var order = _context.Orders.Include(o => o.Customer)
                                       .Include(o => o.Items)
                                       .FirstOrDefault(o => o.SO_ORDER_ID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.Customers = _context.Customers.ToList();
            return View(order);
        }

        [HttpPost]
        public ActionResult Edit(Order model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.ADDRESS))
                    {
                        model.ADDRESS = "";
                    }

                    if (model.SO_ORDER_ID <= 0)
                    {
                        ModelState.AddModelError("", "Invalid Sales Order ID.");
                        ViewBag.Customers = _context.Customers.ToList();
                        return View(model);
                    }

                    var existingOrder = _context.Orders.Include(o => o.Items)
                                                       .FirstOrDefault(o => o.SO_ORDER_ID == model.SO_ORDER_ID);

                    if (existingOrder == null)
                    {
                        ModelState.AddModelError("", "Sales Order not found.");
                        ViewBag.Customers = _context.Customers.ToList();
                        return View(model);
                    }

                    existingOrder.ORDER_NO = model.ORDER_NO;
                    existingOrder.COM_CUSTOMER_ID = model.COM_CUSTOMER_ID;
                    existingOrder.ORDER_DATE = model.ORDER_DATE;
                    existingOrder.ADDRESS = model.ADDRESS;

                    if (model.Items != null && model.Items.Any())
                    {
                        foreach (var item in model.Items)
                        {
                            if (item.SO_ITEM_ID > 0)
                            {
                                var existingItem = existingOrder.Items.FirstOrDefault(i => i.SO_ITEM_ID == item.SO_ITEM_ID);
                                if (existingItem != null)
                                {
                                    existingItem.ITEM_NAME = item.ITEM_NAME;
                                    existingItem.QUANTITY = item.QUANTITY;
                                    existingItem.PRICE = item.PRICE;
                                }
                            }
                            else
                            {
                                var newItem = new Item
                                {
                                    ITEM_NAME = item.ITEM_NAME,
                                    QUANTITY = item.QUANTITY,
                                    PRICE = item.PRICE,
                                    SO_ORDER_ID = existingOrder.SO_ORDER_ID
                                };

                                _context.Items.Add(newItem);
                            }
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    TempData["Success"] = true;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    System.Diagnostics.Debug.WriteLine($"Error while saving Sales Order: {ex.Message}");

                    ModelState.AddModelError("", "An error occurred while saving the data.");
                    ViewBag.Customers = _context.Customers.ToList();
                    return View(model);
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            try
            {
                var order = _context.Orders.Include("Items").FirstOrDefault(o => o.SO_ORDER_ID == id);
                if (order == null)
                {
                    return Json(new { success = false, message = "Order not found." });
                }

                foreach (var item in order.Items.ToList())
                {
                    _context.Items.Remove(item);
                }

                _context.Orders.Remove(order);
                _context.SaveChanges();

                return Json(new { success = true, message = "Order deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteItem(int itemId)
        {
            try
            {
                var item = _context.Items.FirstOrDefault(i => i.SO_ITEM_ID == itemId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Item not found." });
                }

                _context.Items.Remove(item);
                _context.SaveChanges();
                return Json(new { success = true, message = "Item deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ExportToExcel()
        {
            var orders = _context.Orders.Include("Customer").ToList();
            var sb = new StringBuilder();
            sb.AppendLine("No\tSales Order\tOrder Date\tCustomer\tAddress");

            int index = 1;
            foreach (var order in orders)
            {
                sb.AppendLine(
                    $"{index}\t{order.ORDER_NO}\t{order.ORDER_DATE:dd/MM/yyyy}\t{order.Customer.CUSTOMER_NAME}\t{order.ADDRESS}"
                );
                index++;
            }

            string fileName = "SalesOrders.xls";
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/vnd.ms-excel", fileName);
        }
    }
}