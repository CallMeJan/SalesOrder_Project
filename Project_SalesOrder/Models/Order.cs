using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace Project_SalesOrder.Models
{
    [Table("SO_ORDER")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SO_ORDER_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string ORDER_NO { get; set; }

        [Required]
        public DateTime ORDER_DATE { get; set; }

        [Required]
        public int COM_CUSTOMER_ID { get; set; }

        [StringLength(100)]
        public string ADDRESS { get; set; } = "";

        [ForeignKey("COM_CUSTOMER_ID")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}