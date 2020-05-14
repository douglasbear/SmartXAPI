using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__PSD_EmployeeList")]
    public partial class PsdEmployeeList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Employee_Code")]
        [StringLength(200)]
        public string EmployeeCode { get; set; }
        [Column("Employee_Name")]
        [StringLength(300)]
        public string EmployeeName { get; set; }
        [Column("Joining_Date", TypeName = "datetime")]
        public DateTime? JoiningDate { get; set; }
        [StringLength(200)]
        public string Nationality { get; set; }
        [Column("National_ID")]
        [StringLength(100)]
        public string NationalId { get; set; }
        [Column("NExpiry_Date", TypeName = "datetime")]
        public DateTime? NexpiryDate { get; set; }
        [StringLength(200)]
        public string Bank { get; set; }
        [StringLength(200)]
        public string AccountNo { get; set; }
        [Column("Job_Title")]
        [StringLength(200)]
        public string JobTitle { get; set; }
        [StringLength(200)]
        public string Department { get; set; }
        public int? Level { get; set; }
        [StringLength(200)]
        public string Inactive { get; set; }
        [StringLength(200)]
        public string Sex { get; set; }
        [Column("DOB", TypeName = "datetime")]
        public DateTime? Dob { get; set; }
        [Column("Mobile_No")]
        [StringLength(200)]
        public string MobileNo { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        [Column("Marital_Status")]
        [StringLength(200)]
        public string MaritalStatus { get; set; }
        [Column("Passport_No")]
        [StringLength(200)]
        public string PassportNo { get; set; }
        [Column("Passport_Issue_Date", TypeName = "datetime")]
        public DateTime? PassportIssueDate { get; set; }
        [Column("Passport_Exp_Date", TypeName = "datetime")]
        public DateTime? PassportExpDate { get; set; }
        [StringLength(200)]
        public string Teacher { get; set; }
    }
}
