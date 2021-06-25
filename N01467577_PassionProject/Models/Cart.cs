using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace N01467577_PassionProject.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public string CustomerName { get; set; }
        public string date { get; set; }

        public ICollection<Product> Products { get; set; }

    }
    public class CartDto
    {
        [Key]
        public int CartId { get; set; }
        public string CustomerName { get; set; }
        public string date { get; set; }
    }
}