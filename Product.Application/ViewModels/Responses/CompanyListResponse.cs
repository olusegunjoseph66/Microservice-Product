using Product.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.ViewModels.Responses
{
    public class CompanyListResponse
    {
        public List<NameAndCode> Companies { get; set; }
    }
}
