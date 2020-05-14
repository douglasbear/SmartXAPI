using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_ClassFeeSetup")]
    public partial class SchClassFeeSetup
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
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
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }

        [ForeignKey(nameof(NClassId))]
        [InverseProperty(nameof(SchClassType.SchClassFeeSetup))]
        public virtual SchClassType NClass { get; set; }
        [ForeignKey(nameof(NFeeTypeId))]
        [InverseProperty(nameof(SchFeeType.SchClassFeeSetup))]
        public virtual SchFeeType NFeeType { get; set; }
        [ForeignKey(nameof(NFrequencyId))]
        [InverseProperty(nameof(SchFrequency.SchClassFeeSetup))]
        public virtual SchFrequency NFrequency { get; set; }
    }
}
