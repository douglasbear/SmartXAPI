using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeTypeDisp
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
        [Column("N_Amount")]
        [StringLength(30)]
        public string NAmount { get; set; }
        [Column("B_AuthenticationReQuired")]
        public bool? BAuthenticationReQuired { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("X_Direction")]
        [StringLength(50)]
        public string XDirection { get; set; }
        [Column("N_FeeCategoryTypeID")]
        public int? NFeeCategoryTypeId { get; set; }
        [Column("N_Discount")]
        public double? NDiscount { get; set; }
    }
}
