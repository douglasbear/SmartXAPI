using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTrainingAttendanceRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_CourseID")]
        public int NCourseId { get; set; }
        [Column("X_CourseName")]
        [StringLength(500)]
        public string XCourseName { get; set; }
        [Column("X_Venue")]
        [StringLength(200)]
        public string XVenue { get; set; }
        [Required]
        [Column("X_TraineeName")]
        [StringLength(50)]
        public string XTraineeName { get; set; }
        [StringLength(8000)]
        public string StartDate { get; set; }
        [StringLength(8000)]
        public string EndDate { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("I_Employe_Sign", TypeName = "image")]
        public byte[] IEmployeSign { get; set; }
        [Column("X_CourseCode")]
        [StringLength(50)]
        public string XCourseCode { get; set; }
        [Column("N_days")]
        public int? NDays { get; set; }
    }
}
