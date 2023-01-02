using System.ComponentModel;

namespace Product.Application.Enums
{
    public enum ProductStatusEnum
    {
        [Description("Active")]
        Active = 1,

        [Description("InActive")]
        InActive = 2,
    }
}