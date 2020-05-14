using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Course")]
    public partial class PayCourse
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CourseID")]
        public int NCourseId { get; set; }
        [Column("X_CourseCode")]
        [StringLength(50)]
        public string XCourseCode { get; set; }
        [Column("X_CourseName")]
        [StringLength(500)]
        public string XCourseName { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("N_TrainerID")]
        public int? NTrainerId { get; set; }
        [Column("N_days")]
        public int? NDays { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_Capacity")]
        public int? NCapacity { get; set; }
        [Column("X_City")]
        [StringLength(500)]
        public string XCity { get; set; }
        [Column("X_Center")]
        [StringLength(500)]
        public string XCenter { get; set; }
        [Column("X_Address")]
        [StringLength(500)]
        public string XAddress { get; set; }
    }
}
