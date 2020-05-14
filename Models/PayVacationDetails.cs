using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_VacationDetails")]
    public partial class PayVacationDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_VacationID")]
        public int NVacationId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("D_VacReqestDate", TypeName = "datetime")]
        public DateTime? DVacReqestDate { get; set; }
        [Column("D_VacSanctionDate", TypeName = "datetime")]
        public DateTime? DVacSanctionDate { get; set; }
        [Column("D_VacDateFrom", TypeName = "datetime")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("D_VacDateTo", TypeName = "datetime")]
        public DateTime? DVacDateTo { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Column("X_VacRemarks")]
        [StringLength(250)]
        public string XVacRemarks { get; set; }
        [Column("D_VacApprovedDate", TypeName = "datetime")]
        public DateTime? DVacApprovedDate { get; set; }
        [Column("X_VacApprovedComments")]
        [StringLength(250)]
        public string XVacApprovedComments { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
        [Column("N_AssetAssignEmpID")]
        public int? NAssetAssignEmpId { get; set; }
        [Column("N_VacRequestID")]
        public int? NVacRequestId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_VacationStatus")]
        public int? NVacationStatus { get; set; }
        [Column("B_IsAdjustEntry")]
        public bool? BIsAdjustEntry { get; set; }
        [Column("B_Ticket")]
        public bool? BTicket { get; set; }
        [Column("B_ReEntry")]
        public bool? BReEntry { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_ContactNo")]
        [StringLength(50)]
        public string XContactNo { get; set; }
        [Column("N_DelegateID")]
        public int? NDelegateId { get; set; }
        [Column("X_VacationCode")]
        [StringLength(50)]
        public string XVacationCode { get; set; }
        [Column("N_VoucherID")]
        public int? NVoucherId { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("B_Allowance")]
        public bool? BAllowance { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("X_FileName")]
        public string XFileName { get; set; }
        [Column("X_Comments")]
        [StringLength(200)]
        public string XComments { get; set; }
        [Column("N_AgentID")]
        public int? NAgentId { get; set; }
        [Column("N_TravelType")]
        public int? NTravelType { get; set; }
        [Column("D_TravelDate", TypeName = "datetime")]
        public DateTime? DTravelDate { get; set; }
        [Column("N_VacationGroupID")]
        public int? NVacationGroupId { get; set; }
    }
}
