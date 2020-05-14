using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeType")]
    public partial class SchFeeType
    {
        public SchFeeType()
        {
            SchClassFeeSetup = new HashSet<SchClassFeeSetup>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
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
        [Column("N_SiblingCountID")]
        public int? NSiblingCountId { get; set; }
        [Column("B_IsActive")]
        public bool? BIsActive { get; set; }
        [Column("B_IsDiscount")]
        public bool? BIsDiscount { get; set; }
        [Column("B_Reserve")]
        public bool? BReserve { get; set; }
        [Column("B_IsCreditNote")]
        public bool? BIsCreditNote { get; set; }
        [Column("B_IsAdvance")]
        public bool? BIsAdvance { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }

        [ForeignKey(nameof(NFrequencyId))]
        [InverseProperty(nameof(SchFrequency.SchFeeType))]
        public virtual SchFrequency NFrequencyNavigation { get; set; }
        [InverseProperty("NFeeType")]
        public virtual ICollection<SchClassFeeSetup> SchClassFeeSetup { get; set; }
    }
}
