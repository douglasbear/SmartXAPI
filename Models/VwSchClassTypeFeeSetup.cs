using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClassTypeFeeSetup
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ClassFeeID")]
        public int NClassFeeId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("B_IsDefault")]
        public bool? BIsDefault { get; set; }
        [Column("B_IsSelectedFee")]
        public bool? BIsSelectedFee { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Required]
        [Column("X_FrequencyName")]
        [StringLength(15)]
        public string XFrequencyName { get; set; }
        [Column("N_Frequency2")]
        public int NFrequency2 { get; set; }
        [Column("N_YearTotal", TypeName = "money")]
        public decimal? NYearTotal { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }
    }
}
