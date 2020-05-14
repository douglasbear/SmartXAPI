using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeAlerts
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("N_AlertID")]
        public int? NAlertId { get; set; }
        [Column("X_Subject")]
        [StringLength(100)]
        public string XSubject { get; set; }
        [Column("D_AlertDate", TypeName = "datetime")]
        public DateTime? DAlertDate { get; set; }
        [Column("N_Days")]
        public int? NDays { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("X_AlertType")]
        [StringLength(50)]
        public string XAlertType { get; set; }
        [Column("B_DonotShow")]
        public bool? BDonotShow { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_DepartmentID")]
        public int NDepartmentId { get; set; }
    }
}
