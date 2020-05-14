using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchPromotionFeeSetup
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FeeMappingID")]
        public int NFeeMappingId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("N_ClassTypeIDTo")]
        public int? NClassTypeIdto { get; set; }
        [Column("X_ClassTypeTo")]
        [StringLength(50)]
        public string XClassTypeTo { get; set; }
        [Column("N_ClassIDTo")]
        public int? NClassIdto { get; set; }
        [Column("X_ClassTo")]
        [StringLength(50)]
        public string XClassTo { get; set; }
        [Column("N_FeeTypeID")]
        public int? NFeeTypeId { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Column("N_FeeTypeIDTo")]
        public int? NFeeTypeIdto { get; set; }
        [Required]
        [Column("X_FeeTypeTo")]
        [StringLength(50)]
        public string XFeeTypeTo { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_Frequency")]
        public int NFrequency { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
    }
}
