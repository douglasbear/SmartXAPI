using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjTenderDashboard
    {
        [Column("N_TenderID")]
        public int NTenderId { get; set; }
        [Column("X_TenderCode")]
        [StringLength(50)]
        public string XTenderCode { get; set; }
        [Required]
        [Column("X_TenderName")]
        [StringLength(100)]
        public string XTenderName { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("N_PurchaseAmt")]
        [StringLength(30)]
        public string NPurchaseAmt { get; set; }
        [Column("D_SubmissionDate", TypeName = "datetime")]
        public DateTime? DSubmissionDate { get; set; }
        [Column("X_Review")]
        [StringLength(500)]
        public string XReview { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate")]
        [StringLength(8000)]
        public string DEntrydate { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Required]
        [Column("X_EnquiryType")]
        [StringLength(50)]
        public string XEnquiryType { get; set; }
        [Required]
        [Column("X_ProjectType")]
        [StringLength(50)]
        public string XProjectType { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_ReferenceCode")]
        [StringLength(100)]
        public string XReferenceCode { get; set; }
        [Column("X_Status")]
        [StringLength(100)]
        public string XStatus { get; set; }
        [Column("X_Comment")]
        [StringLength(100)]
        public string XComment { get; set; }
        [Column("X_MoreDetails")]
        [StringLength(200)]
        public string XMoreDetails { get; set; }
        [Column("N_ApproxValue", TypeName = "money")]
        public decimal? NApproxValue { get; set; }
        [Column("N_ProcStatus")]
        public int NProcStatus { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_IssaveDraft")]
        public bool NIssaveDraft { get; set; }
        [Column("D_TenderDate", TypeName = "datetime")]
        public DateTime? DTenderDate { get; set; }
        [Column("D_DocumentSubmissionDate")]
        [StringLength(8000)]
        public string DDocumentSubmissionDate { get; set; }
        [Column("N_PerfomanceBondAmount", TypeName = "money")]
        public decimal? NPerfomanceBondAmount { get; set; }
        [Column("N_BidBondAmount")]
        [StringLength(30)]
        public string NBidBondAmount { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_UserName")]
        [StringLength(60)]
        public string XUserName { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_BidStatus")]
        [StringLength(200)]
        public string XBidStatus { get; set; }
        [Column("X_BidRemark")]
        [StringLength(500)]
        public string XBidRemark { get; set; }
        [Column("N_BidBondValidity")]
        public int? NBidBondValidity { get; set; }
        [Column("D_BidBondProcessDate", TypeName = "datetime")]
        public DateTime DBidBondProcessDate { get; set; }
        [Column("X_Notes")]
        [StringLength(200)]
        public string XNotes { get; set; }
        [Column("N_BidBondAmountPerc")]
        public double? NBidBondAmountPerc { get; set; }
        [Column("N_PkeyId")]
        public int? NPkeyId { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [StringLength(50)]
        public string Typename { get; set; }
        [Column("N_SaveDraft")]
        public int? NSaveDraft { get; set; }
        [Column("N_TypeOfSubmission")]
        public int? NTypeOfSubmission { get; set; }
        [Required]
        [Column("X_PlaceOfSubmission")]
        [StringLength(200)]
        public string XPlaceOfSubmission { get; set; }
        [Column("N_BidStatus")]
        public int? NBidStatus { get; set; }
        [Required]
        [Column("X_TypeOfSubmission")]
        [StringLength(50)]
        public string XTypeOfSubmission { get; set; }
        [Required]
        [Column("X_BidStatus2")]
        [StringLength(50)]
        public string XBidStatus2 { get; set; }
        [Column("N_TeamLeaderID")]
        public int NTeamLeaderId { get; set; }
        [Required]
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Required]
        [Column("X_StatusName")]
        [StringLength(50)]
        public string XStatusName { get; set; }
    }
}
