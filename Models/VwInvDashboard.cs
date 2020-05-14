using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvDashboard
    {
        [Column("D_RefDate")]
        [StringLength(50)]
        public string DRefDate { get; set; }
        [Required]
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Required]
        [Column("X_BeneficiaryName")]
        [StringLength(100)]
        public string XBeneficiaryName { get; set; }
        [Required]
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Required]
        [Column("N_Amount")]
        [StringLength(50)]
        public string NAmount { get; set; }
        [Required]
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Required]
        [Column("N_ContractAmt")]
        [StringLength(50)]
        public string NContractAmt { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_StatusID")]
        public int NStatusId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_PaymentStatusID")]
        public int NPaymentStatusId { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Required]
        [Column("X_Draft_User")]
        [StringLength(50)]
        public string XDraftUser { get; set; }
        [Required]
        [Column("X_Draft_BranchName")]
        [StringLength(50)]
        public string XDraftBranchName { get; set; }
        [Required]
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Required]
        [Column("X_User")]
        [StringLength(50)]
        public string XUser { get; set; }
        [Column("N_DraftUser_ID")]
        public int? NDraftUserId { get; set; }
        [Column("N_DraftBranch_ID")]
        public int? NDraftBranchId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
    }
}
