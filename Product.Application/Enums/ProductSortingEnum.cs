using System.ComponentModel;

namespace Product.Application.Enums
{
    public enum ProductSortingEnum
    {
        [Description("name_asc")]
        NameAscending = 1,

        [Description("name_desc")]
        NameDescending = 2,
    }
}
