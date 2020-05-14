using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class Teacher
    {
        [Column("Teacher_Id")]
        public int TeacherId { get; set; }
        [Required]
        [Column("Department_ID")]
        [StringLength(1)]
        public string DepartmentId { get; set; }
        [Column("Teacher_First_Name")]
        [StringLength(100)]
        public string TeacherFirstName { get; set; }
        [Required]
        [Column("Teacher_Middle_Name")]
        [StringLength(1)]
        public string TeacherMiddleName { get; set; }
        [Required]
        [Column("Teacher_Last_Name")]
        [StringLength(1)]
        public string TeacherLastName { get; set; }
        [Column("Teacher_Gender")]
        [StringLength(50)]
        public string TeacherGender { get; set; }
        [Column("TEACHER_DATE_OF_BIRTH", TypeName = "datetime")]
        public DateTime? TeacherDateOfBirth { get; set; }
        [Column("TEACHER_ADDRESS")]
        [StringLength(250)]
        public string TeacherAddress { get; set; }
        [Column("TEACHER_EMAIL")]
        [StringLength(50)]
        public string TeacherEmail { get; set; }
        [Column("TEACHER_BLOOD_GROUP")]
        public int? TeacherBloodGroup { get; set; }
        [Column("TEACHER_PHOTO")]
        public int? TeacherPhoto { get; set; }
        [Column("TEACHER_NATIONALITY")]
        [StringLength(50)]
        public string TeacherNationality { get; set; }
        [Column("TEACHER_PASSPORT")]
        [StringLength(50)]
        public string TeacherPassport { get; set; }
        [Column("TEACHER_PRIMARY_MOBILE_PHONE")]
        [StringLength(50)]
        public string TeacherPrimaryMobilePhone { get; set; }
        [Column("TEACHER_SECONDARY_MOBILE_PHONE")]
        [StringLength(50)]
        public string TeacherSecondaryMobilePhone { get; set; }
        [Column("TEACHER_PRIMARY_RESIDENCE_PHONE")]
        [StringLength(50)]
        public string TeacherPrimaryResidencePhone { get; set; }
        [Column("TEACHER_SECONDARY_RESIDENCE_PHONE")]
        public int? TeacherSecondaryResidencePhone { get; set; }
        [Column("TEACHER_SALARY")]
        public int? TeacherSalary { get; set; }
        [Column("TEACHER_JOINING_DATE", TypeName = "datetime")]
        public DateTime? TeacherJoiningDate { get; set; }
        [Column("TEACHER_QUALIFICATION")]
        [StringLength(50)]
        public string TeacherQualification { get; set; }
        [Column("TEACHER_HIGHEST_DEGREE")]
        public int? TeacherHighestDegree { get; set; }
        [Required]
        [Column("TEACHER_RESUME")]
        [StringLength(1)]
        public string TeacherResume { get; set; }
        [Column("Teacher_User_ID")]
        public int? TeacherUserId { get; set; }
        [Column("Teacher_Username")]
        public int? TeacherUsername { get; set; }
        [Required]
        [Column("Teacher_password")]
        [StringLength(1)]
        public string TeacherPassword { get; set; }
        [Column("Class_ID")]
        public int? ClassId { get; set; }
        [Column("DivisionID")]
        public int? DivisionId { get; set; }
        [Column("Teacher_Remarks")]
        [StringLength(100)]
        public string TeacherRemarks { get; set; }
        [Column("TEACHER_CREATED_DATE", TypeName = "datetime")]
        public DateTime? TeacherCreatedDate { get; set; }
        [Column("TEACHER_Modified_DATE", TypeName = "datetime")]
        public DateTime? TeacherModifiedDate { get; set; }
        [Required]
        [Column("CategoryID")]
        [StringLength(1)]
        public string CategoryId { get; set; }
        [Required]
        [Column("DesignationID")]
        [StringLength(1)]
        public string DesignationId { get; set; }
        [Required]
        [Column("Iquama_no")]
        [StringLength(1)]
        public string IquamaNo { get; set; }
        [Column("Teacher_Code")]
        [StringLength(50)]
        public string TeacherCode { get; set; }
    }
}
