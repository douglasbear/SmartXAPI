using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSupervisor
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SupervisorID")]
        public int NSupervisorId { get; set; }
        [Column("X_SupervisorCode")]
        [StringLength(50)]
        public string XSupervisorCode { get; set; }
        [Column("X_Supervisor")]
        [StringLength(100)]
        public string XSupervisor { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
