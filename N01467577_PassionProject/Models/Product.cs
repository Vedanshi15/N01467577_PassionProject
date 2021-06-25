using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace N01467577_PassionProject.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }

        public bool ProductHasPic { get; set; }
        public string PicExtension { get; set; }
        public ICollection<Cart> Carts { get; set; }
    }
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public bool ProductHasPic { get; set; }
        public string PicExtension { get; set; }

    }
}