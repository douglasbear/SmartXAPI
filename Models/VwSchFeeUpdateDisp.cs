using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeUpdateDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_FeeUpdateID")]
        public int NFeeUpdateId { get; set; }
        [Required]
        [Column("X_FeeUpdateNo")]
        [StringLength(50)]
        public string XFeeUpdateNo { get; set; }
        [Column("N_FeeTypeID")]
        public int? NFeeTypeId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_Amount")]
        [StringLength(20)]
        public string NAmount { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
    }
}
