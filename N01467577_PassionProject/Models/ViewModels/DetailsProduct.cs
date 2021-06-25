using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N01467577_PassionProject.Models.ViewModels
{
    public class DetailsProduct
    {
        public ProductDto SelectedProduct { get; set; }
        public IEnumerable<CartDto> RelatedCart { get; set; }
    }
}