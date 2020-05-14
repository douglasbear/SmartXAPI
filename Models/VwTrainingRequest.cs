using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTrainingRequest
    {
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_RequestID")]
        public int NRequestId { get; set; }
        [Column("X_RequestCode")]
        [StringLength(50)]
        public string XRequestCode { get; set; }
        [Column("N_CourseID")]
        public int NCourseId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [StringLength(8000)]
        public string StartDate { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [StringLength(8000)]
        public string EndDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(300)]
        public string XRemarks { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("X_CourseName")]
        [StringLength(500)]
        public string XCourseName { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_PositionCode")]
        [StringLength(50)]
        public string XPositionCode { get; set; }
        [Column("N_PositionID")]
        public int NPositionId { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Venue")]
        [StringLength(200)]
        public string XVenue { get; set; }
        [Column("N_TrainingTypeID")]
        public int? NTrainingTypeId { get; set; }
        [Column("X_Subject")]
        [StringLength(200)]
        public string XSubject { get; set; }
        [Column("X_TrainingRsn")]
        [StringLength(500)]
        public string XTrainingRsn { get; set; }
        [Column("N_RegFee")]
        public int? NRegFee { get; set; }
        [Column("N_AccomodationFee")]
        public int? NAccomodationFee { get; set; }
        [Column("N_TravelCost")]
        public int? NTravelCost { get; set; }
        [Column("N_Otherfee")]
        public int? NOtherfee { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
    }
}
