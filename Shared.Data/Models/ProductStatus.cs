using System;
using System.Collections.Generic;

namespace Shared.Data.Models
{
    public partial class ProductStatus
    {
        public ProductStatus()
        {
            Products = new HashSet<Product>();
        }

        public byte Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
