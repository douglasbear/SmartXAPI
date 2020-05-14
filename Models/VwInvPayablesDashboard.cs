using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPayablesDashboard
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_Type")]
        [StringLength(25)]
        public string XType { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("N_BalanceAmount")]
        [StringLength(30)]
        public string NBalanceAmount { get; set; }
        [StringLength(30)]
        public string NetAmount { get; set; }
        [StringLength(30)]
        public string PaidAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(50)]
        public string XRemarks { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column(TypeName = "money")]
        public decimal? BalanceAmt { get; set; }
        [Column("X_Code")]
        [StringLength(100)]
        public string XCode { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Required]
        [StringLength(50)]
        public string Balance { get; set; }
        [Column("X_Status")]
        [StringLength(14)]
        public string XStatus { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Required]
        [Column("X_PhoneNo")]
        [StringLength(20)]
        public string XPhoneNo { get; set; }
        [Column("D_delaydays")]
        [StringLength(8000)]
        public string DDelaydays { get; set; }
        [Required]
        [Column("X_VendorInvoice")]
        [StringLength(50)]
        public string XVendorInvoice { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("N_balancehomecurr", TypeName = "money")]
        public decimal? NBalancehomecurr { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
