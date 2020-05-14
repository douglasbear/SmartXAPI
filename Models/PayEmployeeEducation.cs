using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeEducation")]
    public partial class PayEmployeeEducation
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_EduID")]
        public int NEduId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_Course")]
        [StringLength(200)]
        public string XCourse { get; set; }
        [Column("X_Institution")]
        [StringLength(800)]
        public string XInstitution { get; set; }
        [Column("X_Year")]
        [StringLength(100)]
        public string XYear { get; set; }
        [Column("X_Mark")]
        [StringLength(100)]
        public string XMark { get; set; }
    }
}
