using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjMainProjectStock
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_MainProjectID")]
        public int NMainProjectId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Stock")]
        public double? NStock { get; set; }
    }
}
