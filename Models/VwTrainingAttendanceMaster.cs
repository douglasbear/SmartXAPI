using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTrainingAttendanceMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_AttendanceID")]
        public int NAttendanceId { get; set; }
        [Column("X_AttendanceCode")]
        [StringLength(200)]
        public string XAttendanceCode { get; set; }
        [Column("N_CourseID")]
        public int? NCourseId { get; set; }
        [Column("D_TrainingDate", TypeName = "datetime")]
        public DateTime? DTrainingDate { get; set; }
        [Column("X_Description")]
        [StringLength(1000)]
        public string XDescription { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_CourseCode")]
        [StringLength(50)]
        public string XCourseCode { get; set; }
        [Column("X_CourseName")]
        [StringLength(500)]
        public string XCourseName { get; set; }
        [Column("Dt_TrainingDate")]
        public string DtTrainingDate { get; set; }
    }
}
