using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class Student
    {
        [Column("STUDENT_ID")]
        public int? StudentId { get; set; }
        [Column("STUDENT_ADMISSION_NO")]
        [StringLength(50)]
        public string StudentAdmissionNo { get; set; }
        [Column("STUDENT_FIRST_NAME")]
        [StringLength(255)]
        public string StudentFirstName { get; set; }
        [Required]
        [Column("STUDENT_MIDDILE_NAME")]
        [StringLength(1)]
        public string StudentMiddileName { get; set; }
        [Required]
        [Column("STUDENT_LAST_NAME")]
        [StringLength(1)]
        public string StudentLastName { get; set; }
        [Column("STUDENT_GENDER")]
        [StringLength(10)]
        public string StudentGender { get; set; }
        [Column("STUDENT_IMAGE")]
        public int? StudentImage { get; set; }
        [Column("STUDENT_DATE_OF_BIRTH", TypeName = "datetime")]
        public DateTime? StudentDateOfBirth { get; set; }
        [Column("STUDENT_NATIONALITY")]
        [StringLength(100)]
        public string StudentNationality { get; set; }
        [Column("NationalID")]
        [StringLength(100)]
        public string NationalId { get; set; }
        [Column("STUDENT_ADDRESS")]
        [StringLength(500)]
        public string StudentAddress { get; set; }
        [Column("mobileno_1")]
        [StringLength(50)]
        public string Mobileno1 { get; set; }
        [Column("mobileno")]
        [StringLength(50)]
        public string Mobileno { get; set; }
        [Column("phone1")]
        [StringLength(50)]
        public string Phone1 { get; set; }
        [Column("phone2")]
        [StringLength(50)]
        public string Phone2 { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("X_BloodGroup")]
        [StringLength(25)]
        public string XBloodGroup { get; set; }
        [Required]
        [Column("student_passport")]
        [StringLength(1)]
        public string StudentPassport { get; set; }
        [Column("student_userid")]
        public int? StudentUserid { get; set; }
        [Required]
        [Column("student_username")]
        [StringLength(1)]
        public string StudentUsername { get; set; }
        [Required]
        [Column("student_password")]
        [StringLength(1)]
        public string StudentPassword { get; set; }
        [Column("Student_joiningDate", TypeName = "datetime")]
        public DateTime? StudentJoiningDate { get; set; }
        [Column("Class_id")]
        public int? ClassId { get; set; }
        [Column("division_id")]
        public int? DivisionId { get; set; }
        [Column("tescher_id")]
        public int TescherId { get; set; }
        [Column("family_id")]
        public int? FamilyId { get; set; }
        [Column("guardian_id")]
        public int? GuardianId { get; set; }
        [Column("student_remarks")]
        public string StudentRemarks { get; set; }
        [Column("student_alergies")]
        [StringLength(100)]
        public string StudentAlergies { get; set; }
        [Column("physically_chalanged")]
        [StringLength(100)]
        public string PhysicallyChalanged { get; set; }
        [Column("care_requirement")]
        [StringLength(100)]
        public string CareRequirement { get; set; }
        [Column("st_transportation")]
        [StringLength(100)]
        public string StTransportation { get; set; }
        [Required]
        [Column("student_quran")]
        [StringLength(1)]
        public string StudentQuran { get; set; }
        [Column("student_created_date", TypeName = "datetime")]
        public DateTime? StudentCreatedDate { get; set; }
        [Column("student_modified_date", TypeName = "datetime")]
        public DateTime? StudentModifiedDate { get; set; }
        [Column("house_id")]
        public int? HouseId { get; set; }
        [Column("iquama_number")]
        [StringLength(100)]
        public string IquamaNumber { get; set; }
        [Column("Actual_AdmissionNo")]
        [StringLength(50)]
        public string ActualAdmissionNo { get; set; }
    }
}
