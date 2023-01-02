using System;
using System.Collections.Generic;

namespace Shared.Data.Models
{
    public partial class ProductImage
    {
        public int Id { get; set; }
        public short ProductId { get; set; }
        public string PublicUrl { get; set; } = null!;
        public string CloudPath { get; set; } = null!;
        public bool IsPrimaryImage { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
