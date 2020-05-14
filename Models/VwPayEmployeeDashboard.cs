using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeDashboard
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmployeeCode")]
        [StringLength(50)]
        public string XEmployeeCode { get; set; }
        [Column("X_EmployeeName")]
        [StringLength(100)]
        public string XEmployeeName { get; set; }
        [Column("X_PhoneNO")]
        [StringLength(50)]
        public string XPhoneNo { get; set; }
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
        [StringLength(8000)]
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
        [Column("X_Sex")]
        [StringLength(50)]
        public string XSex { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_EmpTypeID")]
        public int? NEmpTypeId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_TicketCount")]
        [StringLength(20)]
        public string NTicketCount { get; set; }
    }
}
