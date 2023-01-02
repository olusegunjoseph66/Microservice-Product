using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.DTOs.APIDataFormatters
{
    public class SwaggerResponse<T> where T : class
    {
        public T Data { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class TestClass<T> where T: class
    {
        public SwaggerResponse<T> Data { get; set; }
    }
}
