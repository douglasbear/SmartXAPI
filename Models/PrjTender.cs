using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_Tender")]
    public partial class PrjTender
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_TenderID")]
        public int NTenderId { get; set; }
        [Column("X_TenderCode")]
        [StringLength(50)]
        public string XTenderCode { get; set; }
        [Column("X_TenderName")]
        [StringLength(100)]
        public string XTenderName { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_EnquiryTypeID")]
        public int? NEnquiryTypeId { get; set; }
        [Column("N_ProjectTypeID")]
        public int? NProjectTypeId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("N_PurchaseAmt", TypeName = "money")]
        public decimal? NPurchaseAmt { get; set; }
        [Column("D_SubmissionDate", TypeName = "datetime")]
        public DateTime? DSubmissionDate { get; set; }
        [Column("X_Review")]
        [StringLength(500)]
        public string XReview { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
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
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_BidBondAmount", TypeName = "money")]
        public decimal? NBidBondAmount { get; set; }
        [Column("N_PerfomanceBondAmount", TypeName = "money")]
        public decimal? NPerfomanceBondAmount { get; set; }
        [Column("D_DocumentSubmissionDate", TypeName = "datetime")]
        public DateTime? DDocumentSubmissionDate { get; set; }
        [Column("B_IssaveDraft")]
        public bool? BIssaveDraft { get; set; }
        [Column("D_TenderDate", TypeName = "datetime")]
        public DateTime? DTenderDate { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("X_BidStatus")]
        [StringLength(200)]
        public string XBidStatus { get; set; }
        [Column("X_BidRemark")]
        [StringLength(500)]
        public string XBidRemark { get; set; }
        [Column("N_BidBondValidity")]
        public int? NBidBondValidity { get; set; }
        [Column("D_BidBondProcessDate", TypeName = "datetime")]
        public DateTime? DBidBondProcessDate { get; set; }
        [Column("X_Notes")]
        [StringLength(200)]
        public string XNotes { get; set; }
        [Column("N_BidBondAmountPerc")]
        public double? NBidBondAmountPerc { get; set; }
        [Column("N_CRMID")]
        public int? NCrmid { get; set; }
        [Column("N_TypeOfSubmission")]
        public int? NTypeOfSubmission { get; set; }
        [Column("N_BidStatus")]
        public int? NBidStatus { get; set; }
        [Column("X_PlaceOfSubmission")]
        [StringLength(200)]
        public string XPlaceOfSubmission { get; set; }
        [Column("N_TeamLeaderID")]
        public int? NTeamLeaderId { get; set; }
        [Column("X_ProjectPlace")]
        [StringLength(500)]
        public string XProjectPlace { get; set; }
        [Column("X_Payee")]
        [StringLength(100)]
        public string XPayee { get; set; }
        [Column("X_ClientContact")]
        [StringLength(50)]
        public string XClientContact { get; set; }
        [Column("X_ProjectReference")]
        [StringLength(200)]
        public string XProjectReference { get; set; }
        [Column("N_ProposedAmt", TypeName = "money")]
        public decimal? NProposedAmt { get; set; }
        [Column("D_EnquiryDate", TypeName = "datetime")]
        public DateTime? DEnquiryDate { get; set; }
    }
}
