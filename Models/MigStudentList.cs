using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Mig_StudentList")]
    public partial class MigStudentList
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
        [Column("X_Section")]
        [StringLength(200)]
        public string XSection { get; set; }
        [Column("X_Class")]
        [StringLength(100)]
        public string XClass { get; set; }
        [Column("X_Division")]
        [StringLength(100)]
        public string XDivision { get; set; }
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
        [Column("X_GuardianNameAr")]
        [StringLength(200)]
        public string XGuardianNameAr { get; set; }
        [Column("X_FatherName")]
        [StringLength(200)]
        public string XFatherName { get; set; }
        [Column("X_FatherNationality")]
        [StringLength(200)]
        public string XFatherNationality { get; set; }
        [Column("X_FatherResID")]
        [StringLength(200)]
        public string XFatherResId { get; set; }
        [Column("X_FatherPassportNo")]
        [StringLength(200)]
        public string XFatherPassportNo { get; set; }
        [Column("X_FatherPhone")]
        [StringLength(200)]
        public string XFatherPhone { get; set; }
        [Column("X_MotherName")]
        [StringLength(200)]
        public string XMotherName { get; set; }
        [Column("X_MotherNationality")]
        [StringLength(200)]
        public string XMotherNationality { get; set; }
        [Column("X_MotherResID")]
        [StringLength(200)]
        public string XMotherResId { get; set; }
        [Column("X_MotherPassportNo")]
        [StringLength(200)]
        public string XMotherPassportNo { get; set; }
        [Column("X_MotherPhone")]
        [StringLength(200)]
        public string XMotherPhone { get; set; }
        [Column("X_ParentAddress")]
        [StringLength(500)]
        public string XParentAddress { get; set; }
        [Column("D_NationalExpFather", TypeName = "datetime")]
        public DateTime? DNationalExpFather { get; set; }
        [Column("D_NationalExpMother", TypeName = "datetime")]
        public DateTime? DNationalExpMother { get; set; }
        [Column("X_Relation")]
        [StringLength(200)]
        public string XRelation { get; set; }
        [Column("D_ReservationDate", TypeName = "datetime")]
        public DateTime? DReservationDate { get; set; }
    }
}
