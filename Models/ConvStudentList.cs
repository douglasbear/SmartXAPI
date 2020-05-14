using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Conv_StudentList")]
    public partial class ConvStudentList
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
        [StringLength(100)]
        public string Guardian { get; set; }
        [Column("id")]
        public int? Id { get; set; }
    }
}
