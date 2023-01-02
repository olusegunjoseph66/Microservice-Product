using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.DTOs
{
    public class SapProductDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ProductType { get; set; } = null!;
        public string ProductStatus { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public string CompanyCode { get; set; } = null!;
        public string UnitOfMeasureCode { get; set; } = null!;
        public string ProductSapNumber { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
