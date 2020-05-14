using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__KES_StudentList")]
    public partial class KesStudentList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Student_Code")]
        [StringLength(200)]
        public string StudentCode { get; set; }
        [Column("Student_Name")]
        [StringLength(300)]
        public string StudentName { get; set; }
        [StringLength(100)]
        public string Gender { get; set; }
        [StringLength(100)]
        public string Nationality { get; set; }
        [Column("DOB", TypeName = "date")]
        public DateTime? Dob { get; set; }
        [Column("NationalityID")]
        [StringLength(100)]
        public string NationalityId { get; set; }
        [StringLength(100)]
        public string Section { get; set; }
        [StringLength(100)]
        public string Class { get; set; }
        [StringLength(100)]
        public string Guardian { get; set; }
        [Column("Mobile_No")]
        [StringLength(100)]
        public string MobileNo { get; set; }
        [Column("NationalId_ExpDate", TypeName = "date")]
        public DateTime? NationalIdExpDate { get; set; }
        [Column("Guardian_Ar")]
        [StringLength(100)]
        public string GuardianAr { get; set; }
    }
}
