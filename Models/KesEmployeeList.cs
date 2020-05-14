using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__KES_EmployeeList")]
    public partial class KesEmployeeList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Employee_Code")]
        [StringLength(200)]
        public string EmployeeCode { get; set; }
        [Column("Employee_Name")]
        [StringLength(300)]
        public string EmployeeName { get; set; }
        [StringLength(100)]
        public string Nationality { get; set; }
        [Column("National_ID")]
        [StringLength(100)]
        public string NationalId { get; set; }
        [Column("National_ExpDate", TypeName = "date")]
        public DateTime? NationalExpDate { get; set; }
        [Column("Job_Title")]
        [StringLength(100)]
        public string JobTitle { get; set; }
        [StringLength(100)]
        public string Department { get; set; }
        [StringLength(100)]
        public string Gender { get; set; }
        [Column("Mobile_No")]
        [StringLength(100)]
        public string MobileNo { get; set; }
        [Column("Email_ID")]
        [StringLength(100)]
        public string EmailId { get; set; }
        [StringLength(100)]
        public string Passport { get; set; }
    }
}
