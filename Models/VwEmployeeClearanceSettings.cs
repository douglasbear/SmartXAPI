using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmployeeClearanceSettings
    {
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_ClearanceSettingsDetailsID")]
        public int NClearanceSettingsDetailsId { get; set; }
        [Column("N_ClearanceSettingsID")]
        public int NClearanceSettingsId { get; set; }
        [Column("X_ClearanceItem")]
        [StringLength(200)]
        public string XClearanceItem { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ClearanceCode")]
        [StringLength(50)]
        public string XClearanceCode { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_ApprovedBy")]
        public int? NApprovedBy { get; set; }
        [Column("X_ApprovedBy")]
        [StringLength(60)]
        public string XApprovedBy { get; set; }
        [Column("X_PurposeName")]
        [StringLength(50)]
        public string XPurposeName { get; set; }
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        [Column("N_PurposeID")]
        public int? NPurposeId { get; set; }
        [Column("B_IsApproved")]
        public bool BIsApproved { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_ApprovedDate", TypeName = "datetime")]
        public DateTime? DApprovedDate { get; set; }
    }
}
