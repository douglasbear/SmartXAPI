using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rec_EmployeeRequestDetails")]
    public partial class RecEmployeeRequestDetails
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_RequestID")]
        public int NRequestId { get; set; }
        [Key]
        [Column("N_RequestDetailID")]
        public int NRequestDetailId { get; set; }
        [Column("X_VacancyName")]
        [StringLength(500)]
        public string XVacancyName { get; set; }
        [Column("N_JobID")]
        public int? NJobId { get; set; }
        [Column("X_CategoryName")]
        [StringLength(500)]
        public string XCategoryName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Qualification")]
        [StringLength(500)]
        public string XQualification { get; set; }
        [Column("N_NoVacancy")]
        public int NNoVacancy { get; set; }
        [Column("N_Salary", TypeName = "money")]
        public decimal? NSalary { get; set; }
        [Column("D_RecStartDate", TypeName = "datetime")]
        public DateTime? DRecStartDate { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
    }
}
