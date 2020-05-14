using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rec_JobVacancy")]
    public partial class RecJobVacancy
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_VacancyID")]
        public int NVacancyId { get; set; }
        [Column("X_VacancyCode")]
        [StringLength(500)]
        public string XVacancyCode { get; set; }
        [Column("X_VacancyName")]
        [StringLength(500)]
        public string XVacancyName { get; set; }
        [Column("X_Qualification")]
        [StringLength(500)]
        public string XQualification { get; set; }
        [Column("N_NoVacancy")]
        public int NNoVacancy { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("N_Salary", TypeName = "money")]
        public decimal? NSalary { get; set; }
        [Column("N_JobID")]
        public int? NJobId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_CategoryName")]
        [StringLength(500)]
        public string XCategoryName { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("B_IssaveDraft")]
        public bool? BIssaveDraft { get; set; }
        [Column("D_RecStartDate", TypeName = "datetime")]
        public DateTime? DRecStartDate { get; set; }
        [Column("B_IsCareer")]
        public bool? BIsCareer { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_RequestID")]
        public int? NRequestId { get; set; }
        [Column("N_RequestDetailID")]
        public int? NRequestDetailId { get; set; }
    }
}
