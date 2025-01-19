using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project_SalesOrder.Models
{
    [Table("SO_ITEM")]
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SO_ITEM_ID { get; set; }

        [Required]
        public long SO_ORDER_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string ITEM_NAME { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int QUANTITY { get; set; }

        [Required]
        public double PRICE { get; set; }

        [ForeignKey("SO_ORDER_ID")]
        public virtual Order Order { get; set; }
    }
}