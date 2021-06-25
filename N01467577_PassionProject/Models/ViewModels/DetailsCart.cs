using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N01467577_PassionProject.Models.ViewModels
{
    public class DetailsCart
    {
        public CartDto SelectedCart { get; set; }
        public IEnumerable<ProductDto> AddedItems { get; set; }
        public IEnumerable<ProductDto> AvailableItems { get; set; }

    }
}