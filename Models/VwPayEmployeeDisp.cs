using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeDisp
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
        [Column("D_HireDate")]
        [StringLength(30)]
        public string DHireDate { get; set; }
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
        public DateTime DIqamaExpiry { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Required]
        [Column("X_EmailID")]
        [StringLength(100)]
        public string XEmailId { get; set; }
        [Required]
        [Column("X_EmpClearanceCode")]
        [StringLength(50)]
        public string XEmpClearanceCode { get; set; }
        [Required]
        [Column("X_ClearanceCode")]
        [StringLength(50)]
        public string XClearanceCode { get; set; }
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Column("B_Active")]
        public bool BActive { get; set; }
        [Required]
        [Column("X_UserCategory")]
        [StringLength(50)]
        public string XUserCategory { get; set; }
        [Column("SecUserID")]
        public int SecUserId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_EmpUserID")]
        public int NEmpUserId { get; set; }
        [Required]
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Required]
        [Column("X_UserName")]
        [StringLength(60)]
        public string XUserName { get; set; }
    }
}
