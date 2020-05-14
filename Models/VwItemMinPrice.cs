using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwItemMinPrice
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column(TypeName = "money")]
        public decimal? MinPrice { get; set; }
    }
}
