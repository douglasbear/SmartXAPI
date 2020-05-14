using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaDashBoard
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_StatusID")]
        public int NStatusId { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("Register_Date", TypeName = "datetime")]
        public DateTime RegisterDate { get; set; }
        [Column("N_RegId")]
        public int NRegId { get; set; }
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Required]
        [Column("X_FileCode")]
        [StringLength(50)]
        public string XFileCode { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Required]
        [Column("X_mobileNo")]
        [StringLength(20)]
        public string XMobileNo { get; set; }
        [Column("X_Email")]
        [StringLength(100)]
        public string XEmail { get; set; }
        [Column("D_Dob")]
        [StringLength(8000)]
        public string DDob { get; set; }
        [Required]
        [Column("X_StatusName")]
        [StringLength(50)]
        public string XStatusName { get; set; }
        [Column("Status_Date")]
        [StringLength(8000)]
        public string StatusDate { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("N_contryid")]
        public int? NContryid { get; set; }
        [Column("N_Nationality")]
        public int? NNationality { get; set; }
        [Required]
        [Column("X_PassportNo")]
        [StringLength(20)]
        public string XPassportNo { get; set; }
        [Column("X_Qualification")]
        [StringLength(100)]
        public string XQualification { get; set; }
        [Column("X_University")]
        [StringLength(100)]
        public string XUniversity { get; set; }
        [Column("X_Profession")]
        [StringLength(100)]
        public string XProfession { get; set; }
        [Column("RegStatus_Date", TypeName = "datetime")]
        public DateTime RegStatusDate { get; set; }
        [Required]
        [Column("X_Place")]
        [StringLength(50)]
        public string XPlace { get; set; }
        [Column("X_Address")]
        [StringLength(200)]
        public string XAddress { get; set; }
        [Column("N_RegistrationID")]
        public int NRegistrationId { get; set; }
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("N_FileStatusID")]
        public int NFileStatusId { get; set; }
        [Required]
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("File_Date", TypeName = "smalldatetime")]
        public DateTime? FileDate { get; set; }
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
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("FIle_statusID")]
        public int? FileStatusId { get; set; }
        [Column("File_StatusName")]
        [StringLength(500)]
        public string FileStatusName { get; set; }
        [Column("X_File_Description")]
        [StringLength(50)]
        public string XFileDescription { get; set; }
        [Column("X_StatusCode")]
        [StringLength(50)]
        public string XStatusCode { get; set; }
        [Column("X_StageName")]
        [StringLength(100)]
        public string XStageName { get; set; }
        [Column("N_TotalContractAmount", TypeName = "money")]
        public decimal? NTotalContractAmount { get; set; }
        [Column("N_Point_EnglishID")]
        public int? NPointEnglishId { get; set; }
        [Column("N_Point_WorkID")]
        public int? NPointWorkId { get; set; }
        [Column("N_Point_QualificationID")]
        public int? NPointQualificationId { get; set; }
        [Column("N_Point_VsaClsValue")]
        public int? NPointVsaClsValue { get; set; }
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
        [Column("N_Point_OtherValue")]
        public int? NPointOtherValue { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_Contry")]
        [StringLength(100)]
        public string XContry { get; set; }
        [Column("X_SpouseName")]
        [StringLength(30)]
        public string XSpouseName { get; set; }
        [Column("D_SpouseDob", TypeName = "smalldatetime")]
        public DateTime? DSpouseDob { get; set; }
        [Column("X_SpouseQualification")]
        [StringLength(200)]
        public string XSpouseQualification { get; set; }
        [Column("X_SpouseUniversity")]
        [StringLength(200)]
        public string XSpouseUniversity { get; set; }
        [Column("X_SpouseProfession")]
        [StringLength(200)]
        public string XSpouseProfession { get; set; }
        [Column("N_BranchID1")]
        public int? NBranchId1 { get; set; }
        [Column("Appointment_Desc")]
        [StringLength(250)]
        public string AppointmentDesc { get; set; }
        [Column("X_ConsultantName")]
        [StringLength(60)]
        public string XConsultantName { get; set; }
        [Column("X_ParalegalName")]
        [StringLength(60)]
        public string XParalegalName { get; set; }
        [Column("N_AuthorityID​")]
        public int? NAuthorityId { get; set; }
        [Column("X_Authority")]
        [StringLength(50)]
        public string XAuthority { get; set; }
        [Column("X_Occupation")]
        [StringLength(200)]
        public string XOccupation { get; set; }
        [Column("X_AnzscoCode")]
        [StringLength(50)]
        public string XAnzscoCode { get; set; }
        [Column("Stage_Description")]
        [StringLength(50)]
        public string StageDescription { get; set; }
        [Column("AppConsultantID")]
        public int? AppConsultantId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Required]
        [Column("D_AppointmentDate")]
        [StringLength(8000)]
        public string DAppointmentDate { get; set; }
        [Column("T_AppoinmentTime", TypeName = "time(5)")]
        public TimeSpan? TAppoinmentTime { get; set; }
        [Column("T_AppointmentToTime", TypeName = "time(5)")]
        public TimeSpan? TAppointmentToTime { get; set; }
        [Column("App_ConsultantCode")]
        [StringLength(50)]
        public string AppConsultantCode { get; set; }
        [StringLength(100)]
        public string AppConsultantName { get; set; }
        [Column("F_LocationID")]
        public int? FLocationId { get; set; }
        [Column("D_FollowUpDate")]
        [StringLength(8000)]
        public string DFollowUpDate { get; set; }
        [Column("T_FollowUpTime", TypeName = "time(5)")]
        public TimeSpan? TFollowUpTime { get; set; }
        [Column("T_FollowUpToTime", TypeName = "time(5)")]
        public TimeSpan? TFollowUpToTime { get; set; }
        [Column("Follow_ConsultantCode")]
        [StringLength(50)]
        public string FollowConsultantCode { get; set; }
        [StringLength(100)]
        public string FollowConsultantName { get; set; }
        [StringLength(11)]
        public string Age { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column("Last_update")]
        [StringLength(11)]
        public string LastUpdate { get; set; }
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
        [Column("X_AllStatus")]
        [StringLength(50)]
        public string XAllStatus { get; set; }
        [Column("B_IsSaveDraft")]
        public bool BIsSaveDraft { get; set; }
        [StringLength(50)]
        public string StateName { get; set; }
        [Column("ConsultantUserID")]
        public int? ConsultantUserId { get; set; }
        [StringLength(50)]
        public string ConsultantUser { get; set; }
        [Column("N_ParalegalUserID")]
        public int? NParalegalUserId { get; set; }
        [Column("N_ParalegalUser")]
        [StringLength(50)]
        public string NParalegalUser { get; set; }
        [Required]
        [StringLength(8000)]
        public string FileFolloupDate { get; set; }
        [Required]
        [StringLength(250)]
        public string FileFolloupNote { get; set; }
        [StringLength(50)]
        public string FollowUpParalegalCode { get; set; }
        [StringLength(100)]
        public string FollowUpParalegal { get; set; }
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
        [Column("App_N_ID")]
        public int? AppNId { get; set; }
        [Column("App_B_IsComplete")]
        public bool? AppBIsComplete { get; set; }
        [Column("N_FileIDJoin")]
        public int? NFileIdjoin { get; set; }
        [Column("D_LastFollowupDate")]
        [StringLength(8000)]
        public string DLastFollowupDate { get; set; }
    }
}
