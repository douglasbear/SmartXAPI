using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvReceivablesDashboard
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("N_PArtyID")]
        public int? NPartyId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_BalanceAmount")]
        [StringLength(30)]
        public string NBalanceAmount { get; set; }
        [StringLength(30)]
        public string NetAmount { get; set; }
        [StringLength(30)]
        public string PaidAmount { get; set; }
        public int Expr1 { get; set; }
        [Column("D_SalesDate")]
        [StringLength(8000)]
        public string DSalesDate { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Required]
        [Column("N_DiscountAmt")]
        [StringLength(1)]
        public string NDiscountAmt { get; set; }
        [Required]
        [Column("N_Amount")]
        [StringLength(1)]
        public string NAmount { get; set; }
        [Required]
        [Column("N_AmtPaidFromAdvance")]
        [StringLength(1)]
        public string NAmtPaidFromAdvance { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(1)]
        public string XDescription { get; set; }
        [Column("X_TypeCode")]
        [StringLength(30)]
        public string XTypeCode { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Required]
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(14)]
        public string XStatus { get; set; }
        [Column("N_Balance", TypeName = "money")]
        public decimal? NBalance { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(20)]
        public string XPhoneNo { get; set; }
        [Column("D_delaydays")]
        [StringLength(8000)]
        public string DDelaydays { get; set; }
        [Required]
        [Column("X_CustPONo")]
        [StringLength(50)]
        public string XCustPono { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
