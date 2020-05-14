using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchSectionFee
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Required]
        [Column("X_FrequencyName")]
        [StringLength(15)]
        public string XFrequencyName { get; set; }
        [Column("N_Frequency")]
        public int NFrequency { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("B_AuthenticationReQuired")]
        public bool? BAuthenticationReQuired { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("X_FeeCategory")]
        [StringLength(50)]
        public string XFeeCategory { get; set; }
    }
}
