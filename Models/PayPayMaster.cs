using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PayMaster")]
    public partial class PayPayMaster
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
        [Column("N_ParentPayCodeID")]
        public int? NParentPayCodeId { get; set; }
        [Column("X_PayGroup")]
        [StringLength(50)]
        public string XPayGroup { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("B_AutoIncludePayrun")]
        public bool? BAutoIncludePayrun { get; set; }
        [Column("X_Method")]
        [StringLength(100)]
        public string XMethod { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }
        [Column("B_Editable")]
        public bool? BEditable { get; set; }
        [Column("N_PayType")]
        public int? NPayType { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("N_ConfigLevel")]
        public int? NConfigLevel { get; set; }
        [Column("X_DescriptionLocale")]
        [StringLength(100)]
        public string XDescriptionLocale { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_Value2", TypeName = "money")]
        public decimal? NValue2 { get; set; }
        [Column("N_ExpenseDefAccountID")]
        public int? NExpenseDefAccountId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PaymentId")]
        public int? NPaymentId { get; set; }
        [Column("N_MinValue")]
        public double? NMinValue { get; set; }
        [Column("N_MaxValue")]
        public double? NMaxValue { get; set; }
        [Column("N_PayableDefAccountID")]
        public int? NPayableDefAccountId { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }
        [Column("N_SystemType")]
        public int? NSystemType { get; set; }
        [Column("B_IncludeInInvoice")]
        public bool? BIncludeInInvoice { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("B_IsEditableAmt")]
        public bool? BIsEditableAmt { get; set; }
        [Column("B_IsPrepaid")]
        public bool? BIsPrepaid { get; set; }
        [Column("N_NoOfMonths")]
        public int? NNoOfMonths { get; set; }
        [Column("B_IsAnytimeInvoice")]
        public bool? BIsAnytimeInvoice { get; set; }
        [Column("B_IsBasedOnWorkDays")]
        public bool? BIsBasedOnWorkDays { get; set; }
        [Column("B_IsVATRequired")]
        public bool? BIsVatrequired { get; set; }
        [Column("B_IsProjectBased")]
        public bool? BIsProjectBased { get; set; }
        [Column("B_IsPayable")]
        public bool? BIsPayable { get; set; }
        [Column("B_IsReceivable")]
        public bool? BIsReceivable { get; set; }
        [Column("N_TaxID")]
        public int? NTaxId { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("B_IncludeInPayslip")]
        public bool? BIncludeInPayslip { get; set; }
        [Column("B_Amortized")]
        public bool? BAmortized { get; set; }
        [Column("N_IncomeLedgerID")]
        public int? NIncomeLedgerId { get; set; }
        [Column("N_ExpenseLedgerID")]
        public int? NExpenseLedgerId { get; set; }
        [Column("N_TaxPerc")]
        public int? NTaxPerc { get; set; }
        [Column("X_TaxCategory")]
        [StringLength(100)]
        public string XTaxCategory { get; set; }
        [Column("N_AmrIncLedgerID")]
        public int? NAmrIncLedgerId { get; set; }
        [Column("N_AmrExpLedgerID")]
        public int? NAmrExpLedgerId { get; set; }
    }
}
