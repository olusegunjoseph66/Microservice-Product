using System;
using System.Collections.Generic;

namespace Shared.Data.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductImages = new HashSet<ProductImage>();
        }

        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ProductType { get; set; } = null!;
        public byte ProductStatusId { get; set; }
        public string CountryCode { get; set; } = null!;
        public string CompanyCode { get; set; } = null!;
        public string UnitOfMeasureCode { get; set; } = null!;
        public string ProductSapNumber { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime? DateRefreshed { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedByUserId { get; set; }

        public virtual ProductStatus ProductStatus { get; set; } = null!;
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
