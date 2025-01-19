using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project_SalesOrder.Models
{
    [Table("COM_CUSTOMER")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int COM_CUSTOMER_ID { get; set; }

        [StringLength(100)]
        public string CUSTOMER_NAME { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}