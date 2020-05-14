using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCurrentStock
    {
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        public double CurrStock { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
