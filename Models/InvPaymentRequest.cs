using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PaymentRequest")]
    public partial class InvPaymentRequest
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_RequestID")]
        public int NRequestId { get; set; }
        [Column("X_RequestCode")]
        [StringLength(50)]
        public string XRequestCode { get; set; }
        [Column("X_RequestType")]
        [StringLength(100)]
        public string XRequestType { get; set; }
        [Column("N_ApprovedUserID")]
        public int? NApprovedUserId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_TransNo")]
        [StringLength(50)]
        public string XTransNo { get; set; }
        [Column("D_DelDate", TypeName = "datetime")]
        public DateTime? DDelDate { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Column("N_NetAmount", TypeName = "money")]
        public decimal? NNetAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_PartyName")]
        [StringLength(500)]
        public string XPartyName { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_RequestTypeID")]
        public int? NRequestTypeId { get; set; }
        [Column("B_IsDirectEntry")]
        public bool? BIsDirectEntry { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
        [Column("N_ApprovalLevelID")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_IssaveDraft")]
        public bool? BIssaveDraft { get; set; }
        [Column("X_PaymentRefNo")]
        [StringLength(100)]
        public string XPaymentRefNo { get; set; }
        [Column("X_PayTo")]
        [StringLength(200)]
        public string XPayTo { get; set; }
        [Column("X_Beneficiary")]
        [StringLength(200)]
        public string XBeneficiary { get; set; }
        [Column("X_Comments")]
        [StringLength(200)]
        public string XComments { get; set; }
        [Column("N_PaymentFrom")]
        public int? NPaymentFrom { get; set; }
        [Column("N_AppFormID")]
        public int? NAppFormId { get; set; }
    }
}
