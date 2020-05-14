using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_TrainingAttendance")]
    public partial class PayTrainingAttendance
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Key]
        [Column("N_AttendanceID")]
        public int NAttendanceId { get; set; }
        [Column("X_AttendanceCode")]
        [StringLength(200)]
        public string XAttendanceCode { get; set; }
        [Column("N_CourseID")]
        public int? NCourseId { get; set; }
        [Column("D_TrainingDate", TypeName = "datetime")]
        public DateTime? DTrainingDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Description")]
        [StringLength(1000)]
        public string XDescription { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
    }
}
