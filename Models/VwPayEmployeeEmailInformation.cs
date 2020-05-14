using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeEmailInformation
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_ManagerEmpId")]
        public int NManagerEmpId { get; set; }
        [Column("X_ManagerEmpName")]
        [StringLength(100)]
        public string XManagerEmpName { get; set; }
        [Column("X_EmployeeEmailID")]
        [StringLength(50)]
        public string XEmployeeEmailId { get; set; }
        [Column("X_ManagerEmailID")]
        [StringLength(50)]
        public string XManagerEmailId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
    }
}
