using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeMaintenanceDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("Employee Code")]
        [StringLength(50)]
        public string EmployeeCode { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [Column("X_DefEmpCode")]
        [StringLength(50)]
        public string XDefEmpCode { get; set; }
        [Column("X_Phone1")]
        [StringLength(50)]
        public string XPhone1 { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_ReportToID")]
        public int? NReportToId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        [Column("D_HireDate", TypeName = "datetime")]
        public DateTime? DHireDate { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CatagoryId")]
        public int? NCatagoryId { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Column("D_IqamaExpiry", TypeName = "datetime")]
        public DateTime? DIqamaExpiry { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Required]
        [Column("X_EmailID")]
        [StringLength(100)]
        public string XEmailId { get; set; }
    }
}
