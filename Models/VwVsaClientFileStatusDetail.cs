using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaClientFileStatusDetail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Date", TypeName = "smalldatetime")]
        public DateTime DDate { get; set; }
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("N_FileStatusID")]
        public int? NFileStatusId { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("N_RegId")]
        public int? NRegId { get; set; }
        [Column("N_OccupationID")]
        public int? NOccupationId { get; set; }
        [Column("N_AssessmentID")]
        public int? NAssessmentId { get; set; }
        [Column("N_VisaSubClassID")]
        public int? NVisaSubClassId { get; set; }
        [Column("N_StatesID")]
        public int? NStatesId { get; set; }
        [Column("X_StatusNote")]
        [StringLength(200)]
        public string XStatusNote { get; set; }
        [Column("N_ConsultantID")]
        public int? NConsultantId { get; set; }
        [Column("N_ParalegalID")]
        public int? NParalegalId { get; set; }
        [Column("N_Point_EnglishID")]
        public int? NPointEnglishId { get; set; }
        [Column("N_Point_WorkID")]
        public int? NPointWorkId { get; set; }
        [Column("N_Point_QualificationID")]
        public int? NPointQualificationId { get; set; }
        [Column("N_Point_VsaClsID")]
        public int? NPointVsaClsId { get; set; }
        [Column("N_Point_OtherID")]
        public int? NPointOtherId { get; set; }
        [Column("N_Point_AgeValue")]
        public int? NPointAgeValue { get; set; }
        [Column("N_Point_EnglishValue")]
        public int? NPointEnglishValue { get; set; }
        [Column("N_Point_WorkValue")]
        public int? NPointWorkValue { get; set; }
        [Column("N_Point_QualificationValue")]
        public int? NPointQualificationValue { get; set; }
        [Column("N_Point_VsaClsValue")]
        public int? NPointVsaClsValue { get; set; }
        [Column("N_Point_OtherValue")]
        public int? NPointOtherValue { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("X_StatusCode")]
        [StringLength(50)]
        public string XStatusCode { get; set; }
        [Column("FIleStageID")]
        public int? FileStageId { get; set; }
        [Column("X_StageName")]
        [StringLength(100)]
        public string XStageName { get; set; }
        [Column("Stage_Description")]
        [StringLength(50)]
        public string StageDescription { get; set; }
        [Column("X_ConsultantCode")]
        [StringLength(25)]
        public string XConsultantCode { get; set; }
        [Column("X_ConsultantName")]
        [StringLength(60)]
        public string XConsultantName { get; set; }
        [Column("X_ParalegalCode")]
        [StringLength(25)]
        public string XParalegalCode { get; set; }
        [Column("X_ParalegalName")]
        [StringLength(60)]
        public string XParalegalName { get; set; }
        [Column("X_Occupation")]
        [StringLength(200)]
        public string XOccupation { get; set; }
        [Column("N_AuthorityID​")]
        public int? NAuthorityId { get; set; }
        [Column("X_Authority")]
        [StringLength(50)]
        public string XAuthority { get; set; }
        public int? Expr1 { get; set; }
        [Column("X_AnzscoCode")]
        [StringLength(50)]
        public string XAnzscoCode { get; set; }
        [Column("FileStatus_Date", TypeName = "datetime")]
        public DateTime? FileStatusDate { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column("EngPointID")]
        public int? EngPointId { get; set; }
        [StringLength(50)]
        public string EngPointName { get; set; }
        [Column("WorkPointID")]
        public int? WorkPointId { get; set; }
        [StringLength(50)]
        public string WorkPointName { get; set; }
        [Column("QaliPointID")]
        public int? QaliPointId { get; set; }
        [StringLength(50)]
        public string QaliPointName { get; set; }
        [Column("OtherPointID")]
        public int? OtherPointId { get; set; }
        [StringLength(50)]
        public string OtherPointName { get; set; }
        [StringLength(50)]
        public string SubClassName { get; set; }
        [Column("SubClassID")]
        public int? SubClassId { get; set; }
        public int? SubClassPoint { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [StringLength(50)]
        public string StateName { get; set; }
        [Column("N_ParalegalUserID")]
        public int? NParalegalUserId { get; set; }
        [Column("N_ParalegalUser")]
        [StringLength(50)]
        public string NParalegalUser { get; set; }
        [Column("App_ID")]
        public int? AppId { get; set; }
        [StringLength(50)]
        public string FollowUpParalegalCode { get; set; }
        [StringLength(100)]
        public string FollowUpParalegal { get; set; }
        [StringLength(8000)]
        public string FileFolloupDate { get; set; }
        [StringLength(250)]
        public string FileFolloupNote { get; set; }
        [Column("B_IsComplete")]
        public bool? BIsComplete { get; set; }
        [Column("X_Note1")]
        [StringLength(500)]
        public string XNote1 { get; set; }
        [Column("X_Note2")]
        [StringLength(250)]
        public string XNote2 { get; set; }
        [Column("X_Note3")]
        [StringLength(250)]
        public string XNote3 { get; set; }
        [Column("X_VsaTRNno")]
        [StringLength(50)]
        public string XVsaTrnno { get; set; }
        [Column("X_VsaBCC")]
        [StringLength(50)]
        public string XVsaBcc { get; set; }
        [Column("N_MainStatus")]
        public int NMainStatus { get; set; }
        [Column("N_TotalContractAmount", TypeName = "money")]
        public decimal? NTotalContractAmount { get; set; }
    }
}
