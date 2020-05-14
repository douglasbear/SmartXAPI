using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssetPofit
    {
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
    }
}
