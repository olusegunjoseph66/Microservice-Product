using Product.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.ViewModels.Responses
{
    public class ProductDetailDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductType { get; set; }
        public decimal Price { get; set; }
        public string ProductSapNumber { get; set; }
        public NameAndCode UnitOfMeasure { get; set; }
        public NameAndCode ProductStatus { get; set; }
        public DateTime? DateModified { get; set; }
        public List<ProductImageResponse> ProductImages { get; set; }
    }
}
