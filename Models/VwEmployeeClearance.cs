using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmployeeClearance
    {
        [Column("X_ClearanceCode")]
        [StringLength(20)]
        public string XClearanceCode { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ClearanceID")]
        public int NClearanceId { get; set; }
        [Column("D_RequestDate")]
        [StringLength(30)]
        public string DRequestDate { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("D_HireDate")]
        [StringLength(30)]
        public string DHireDate { get; set; }
        [Required]
        [Column("X_Purpose")]
        [StringLength(50)]
        public string XPurpose { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_ClearanceSettingsDetailsID")]
        public int NClearanceSettingsDetailsId { get; set; }
        [Column("N_ClearanceSettingsID")]
        public int NClearanceSettingsId { get; set; }
        [Column("B_IsApproved")]
        public bool? BIsApproved { get; set; }
        [Column("X_ClearanceItem")]
        [StringLength(200)]
        public string XClearanceItem { get; set; }
        [Required]
        [Column("X_ApprovedBy")]
        [StringLength(60)]
        public string XApprovedBy { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_ApprovedBy")]
        public int? NApprovedBy { get; set; }
        [Column("D_ApprovedDate", TypeName = "datetime")]
        public DateTime? DApprovedDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("I_Employe_Sign", TypeName = "image")]
        public byte[] IEmployeSign { get; set; }
    }
}
