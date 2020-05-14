using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchPromotionDisp1
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("Process No")]
        [StringLength(30)]
        public string ProcessNo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("From Section")]
        [StringLength(50)]
        public string FromSection { get; set; }
        [Column("From Class")]
        [StringLength(50)]
        public string FromClass { get; set; }
        [Column("To Section")]
        [StringLength(50)]
        public string ToSection { get; set; }
        [Column("To Class")]
        [StringLength(50)]
        public string ToClass { get; set; }
        [Column("X_PromotionCode")]
        [StringLength(50)]
        public string XPromotionCode { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
    }
}
