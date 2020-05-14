using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjTimesheet
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_TimeSheetID")]
        public int NTimeSheetId { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_TimeSheetReqID")]
        public int NTimeSheetReqId { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_Hours")]
        public double? NHours { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_ServiceID")]
        public int? NServiceId { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
    }
}
