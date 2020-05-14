using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaRegistrationStatusDetail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_RegId")]
        public int NRegId { get; set; }
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Column("X_Place")]
        [StringLength(50)]
        public string XPlace { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_mobileNo")]
        [StringLength(20)]
        public string XMobileNo { get; set; }
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
        [Column("X_Email")]
        [StringLength(30)]
        public string XEmail { get; set; }
        [Column("X_Address")]
        [StringLength(200)]
        public string XAddress { get; set; }
        [Column("X_SpouseName")]
        [StringLength(30)]
        public string XSpouseName { get; set; }
        [Column("D_SpouseDob", TypeName = "smalldatetime")]
        public DateTime? DSpouseDob { get; set; }
        [Column("X_SpouseQualification")]
        [StringLength(200)]
        public string XSpouseQualification { get; set; }
        [Column("D_Dob", TypeName = "smalldatetime")]
        public DateTime DDob { get; set; }
        [Column("X_SpouseUniversity")]
        [StringLength(200)]
        public string XSpouseUniversity { get; set; }
        [Column("X_SpouseProfession")]
        [StringLength(200)]
        public string XSpouseProfession { get; set; }
        [Column("X_StatusDescription")]
        [StringLength(500)]
        public string XStatusDescription { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_RegistrationID")]
        public int NRegistrationId { get; set; }
        [Column("N_StatusID")]
        public int NStatusId { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("Status_Date", TypeName = "datetime")]
        public DateTime? StatusDate { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_Contry")]
        [StringLength(100)]
        public string XContry { get; set; }
        [Column("N_ConsultantID")]
        public int? NConsultantId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("D_AppointmentDate", TypeName = "datetime")]
        public DateTime? DAppointmentDate { get; set; }
        [Column("T_AppoinmentTime", TypeName = "time(5)")]
        public TimeSpan? TAppoinmentTime { get; set; }
        [Column("T_AppointmentToTime", TypeName = "time(5)")]
        public TimeSpan? TAppointmentToTime { get; set; }
        [Column("X_ConsultantCode")]
        [StringLength(25)]
        public string XConsultantCode { get; set; }
        [Column("App_ID")]
        public int? AppId { get; set; }
        [Column("X_ConsultantName")]
        [StringLength(60)]
        public string XConsultantName { get; set; }
        [Column("Appointment_Desc")]
        [StringLength(250)]
        public string AppointmentDesc { get; set; }
        [Column("F_LocationID")]
        public int? FLocationId { get; set; }
        [Column("D_FollowUpDate", TypeName = "datetime")]
        public DateTime? DFollowUpDate { get; set; }
        [Column("T_FollowUpTime", TypeName = "time(5)")]
        public TimeSpan? TFollowUpTime { get; set; }
        [Column("T_FollowUpToTime", TypeName = "time(5)")]
        public TimeSpan? TFollowUpToTime { get; set; }
        [Column("F_ConsultantCode")]
        [StringLength(25)]
        public string FConsultantCode { get; set; }
        [Column("F_ConsultantName")]
        [StringLength(60)]
        public string FConsultantName { get; set; }
        [Column("App_F_ID")]
        public int? AppFId { get; set; }
        [Column("Followup_Desc")]
        [StringLength(250)]
        public string FollowupDesc { get; set; }
        [Column("B_IsComplete")]
        public bool? BIsComplete { get; set; }
        [Column("ConsultantUserID")]
        public int? ConsultantUserId { get; set; }
        [StringLength(50)]
        public string ConsultantUser { get; set; }
    }
}
