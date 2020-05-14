using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__PSD_StudentList2")]
    public partial class PsdStudentList2
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("X_AdmissionNo")]
        [StringLength(200)]
        public string XAdmissionNo { get; set; }
        [Column("D_AdmissionDate", TypeName = "datetime")]
        public DateTime? DAdmissionDate { get; set; }
        [Column("X_LastName")]
        [StringLength(200)]
        public string XLastName { get; set; }
        [Column("X_GivenName")]
        [StringLength(200)]
        public string XGivenName { get; set; }
        [Column("X_MiddleName")]
        [StringLength(200)]
        public string XMiddleName { get; set; }
        [Column("X_Initial")]
        [StringLength(100)]
        public string XInitial { get; set; }
        [Column("X_StudentNameAr")]
        [StringLength(200)]
        public string XStudentNameAr { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("X_Gender")]
        [StringLength(100)]
        public string XGender { get; set; }
        [Column("X_ClassID")]
        [StringLength(100)]
        public string XClassId { get; set; }
        [Column("X_DivisionID")]
        [StringLength(100)]
        public string XDivisionId { get; set; }
        [Column("X_Section")]
        [StringLength(200)]
        public string XSection { get; set; }
        [Column("X_Nationality")]
        [StringLength(200)]
        public string XNationality { get; set; }
        [Column("X_NationalID")]
        [StringLength(200)]
        public string XNationalId { get; set; }
        [Column("X_PassportNo")]
        [StringLength(200)]
        public string XPassportNo { get; set; }
        [Column("X_GuardianName")]
        [StringLength(200)]
        public string XGuardianName { get; set; }
        [Column("X_FatherName")]
        [StringLength(200)]
        public string XFatherName { get; set; }
        [Column("X_MotherName")]
        [StringLength(200)]
        public string XMotherName { get; set; }
        [Column("X_GuardianNameAr")]
        [StringLength(200)]
        public string XGuardianNameAr { get; set; }
        [Column("X_Relation")]
        [StringLength(200)]
        public string XRelation { get; set; }
        [Column("X_FatherPhoneNo")]
        [StringLength(200)]
        public string XFatherPhoneNo { get; set; }
        [Column("X_MotherPhoneNo")]
        [StringLength(200)]
        public string XMotherPhoneNo { get; set; }
        [Column("X_FatherResID")]
        [StringLength(200)]
        public string XFatherResId { get; set; }
        [Column("X_MotherResID")]
        [StringLength(200)]
        public string XMotherResId { get; set; }
        [Column("X_PlaceOfBirth")]
        [StringLength(200)]
        public string XPlaceOfBirth { get; set; }
        [Column("X_LastSchoolAttended")]
        [StringLength(200)]
        public string XLastSchoolAttended { get; set; }
        [Column("X_LastLevel")]
        [StringLength(200)]
        public string XLastLevel { get; set; }
        [Column("X_LastSchoolAddress")]
        [StringLength(200)]
        public string XLastSchoolAddress { get; set; }
        [Column("X_LastGrade")]
        [StringLength(200)]
        public string XLastGrade { get; set; }
        [Column("D_ReservationDate", TypeName = "datetime")]
        public DateTime? DReservationDate { get; set; }
        [Column("X_FatherNationality")]
        [StringLength(200)]
        public string XFatherNationality { get; set; }
        [Column("X_FatherCompany")]
        [StringLength(200)]
        public string XFatherCompany { get; set; }
        [Column("X_Occupation")]
        [StringLength(200)]
        public string XOccupation { get; set; }
        [Column("X_FatherOfficeNo")]
        [StringLength(200)]
        public string XFatherOfficeNo { get; set; }
        [Column("X_FatherEmail")]
        [StringLength(200)]
        public string XFatherEmail { get; set; }
        [Column("X_MotherNationality")]
        [StringLength(200)]
        public string XMotherNationality { get; set; }
        [Column("X_MotherCompany")]
        [StringLength(200)]
        public string XMotherCompany { get; set; }
        [Column("X_MotherOccupation")]
        [StringLength(200)]
        public string XMotherOccupation { get; set; }
        [Column("X_MotherOfficeNo")]
        [StringLength(200)]
        public string XMotherOfficeNo { get; set; }
        [Column("X_MotherEmail")]
        [StringLength(200)]
        public string XMotherEmail { get; set; }
        [Column("X_HomeNo")]
        [StringLength(200)]
        public string XHomeNo { get; set; }
        [Column("X_SysPreviousYear")]
        [StringLength(200)]
        public string XSysPreviousYear { get; set; }
        [Column("X_SchoolAdd")]
        [StringLength(200)]
        public string XSchoolAdd { get; set; }
        [Column("X_BridgeProgram")]
        [StringLength(200)]
        public string XBridgeProgram { get; set; }
        [Column("X_GradePrevious")]
        [StringLength(200)]
        public string XGradePrevious { get; set; }
        [Column("X_ABM_Stem")]
        [StringLength(200)]
        public string XAbmStem { get; set; }
        [Column("X_Status")]
        [StringLength(200)]
        public string XStatus { get; set; }
    }
}
