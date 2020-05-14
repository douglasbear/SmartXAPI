using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Charle_EmployeeList")]
    public partial class CharleEmployeeList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Employee_Code")]
        [StringLength(200)]
        public string EmployeeCode { get; set; }
        [Column("Employee_FirstName")]
        [StringLength(300)]
        public string EmployeeFirstName { get; set; }
        [Column("Employee_LastName")]
        [StringLength(300)]
        public string EmployeeLastName { get; set; }
        [Column("DOB", TypeName = "datetime")]
        public DateTime? Dob { get; set; }
        [StringLength(200)]
        public string Sex { get; set; }
        [Column("Marital_Status")]
        [StringLength(200)]
        public string MaritalStatus { get; set; }
        [StringLength(500)]
        public string Occupation { get; set; }
        [StringLength(200)]
        public string Nationality { get; set; }
        [Column("National_ID")]
        [StringLength(100)]
        public string NationalId { get; set; }
        [StringLength(200)]
        public string Contract { get; set; }
    }
}
