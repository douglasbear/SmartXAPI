using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Charle_StudentList")]
    public partial class CharleStudentList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Student_Code")]
        [StringLength(200)]
        public string StudentCode { get; set; }
        [Column("Student_NameAr")]
        [StringLength(500)]
        public string StudentNameAr { get; set; }
        [Column("Student_Name")]
        [StringLength(500)]
        public string StudentName { get; set; }
        [StringLength(100)]
        public string Gender { get; set; }
        [StringLength(100)]
        public string Nationality { get; set; }
        [Column("DOB", TypeName = "date")]
        public DateTime? Dob { get; set; }
        [StringLength(100)]
        public string Class { get; set; }
        [StringLength(100)]
        public string Division { get; set; }
        [Column("Join_Date", TypeName = "date")]
        public DateTime? JoinDate { get; set; }
        [Column("Guardian_Number")]
        [StringLength(100)]
        public string GuardianNumber { get; set; }
        [Column("Guardian_NameAr")]
        [StringLength(100)]
        public string GuardianNameAr { get; set; }
        [Column("Guardian_Relation")]
        [StringLength(100)]
        public string GuardianRelation { get; set; }
        [Column("Guardian_Mobile_No")]
        [StringLength(100)]
        public string GuardianMobileNo { get; set; }
        [StringLength(100)]
        public string Employer { get; set; }
    }
}
