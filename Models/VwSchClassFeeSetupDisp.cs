using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClassFeeSetupDisp
    {
        [Column("N_ClassFeeID")]
        public int NClassFeeId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("Class Type")]
        [StringLength(50)]
        public string ClassType { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Required]
        [Column("Fee Type")]
        [StringLength(50)]
        public string FeeType { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Required]
        [StringLength(15)]
        public string Frequency { get; set; }
        [StringLength(20)]
        public string Amount { get; set; }
        [Column("B_IsDefault")]
        public bool? BIsDefault { get; set; }
        [Column("B_IsSelectedFee")]
        public bool? BIsSelectedFee { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
