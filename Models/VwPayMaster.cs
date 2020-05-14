using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PayID")]
        public int NPayId { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("X_PayType")]
        [StringLength(100)]
        public string XPayType { get; set; }
        [Column("N_ParentPayCodeID")]
        public int? NParentPayCodeId { get; set; }
        [Required]
        [Column("X_ParentPayCode")]
        [StringLength(50)]
        public string XParentPayCode { get; set; }
        [Column("X_PayGroup")]
        [StringLength(50)]
        public string XPayGroup { get; set; }
        [Column("B_AutoIncludePayrun")]
        public bool? BAutoIncludePayrun { get; set; }
        [Column("X_Method")]
        [StringLength(100)]
        public string XMethod { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("N_PayType")]
        public int? NPayType { get; set; }
        [Column("N_ConfigLevel")]
        public int? NConfigLevel { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PaymentId")]
        public int? NPaymentId { get; set; }
        [Column("N_MinValue")]
        public double NMinValue { get; set; }
        [Column("N_MaxValue")]
        public double NMaxValue { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }
        [Column("B_IncludeInInvoice")]
        public bool BIncludeInInvoice { get; set; }
        [Column("B_IsEditableAmt")]
        public bool BIsEditableAmt { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
    }
}
