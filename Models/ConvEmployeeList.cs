using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Conv_EmployeeList")]
    public partial class ConvEmployeeList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Employee_Code")]
        [StringLength(200)]
        public string EmployeeCode { get; set; }
        [Column("Employee_Name")]
        [StringLength(300)]
        public string EmployeeName { get; set; }
        [Column("Join_Date", TypeName = "date")]
        public DateTime? JoinDate { get; set; }
        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(100)]
        public string Nationality { get; set; }
        [StringLength(100)]
        public string Position { get; set; }
        [StringLength(100)]
        public string Department { get; set; }
        [StringLength(100)]
        public string Gender { get; set; }
        [Column("Mobile_No")]
        [StringLength(100)]
        public string MobileNo { get; set; }
    }
}
