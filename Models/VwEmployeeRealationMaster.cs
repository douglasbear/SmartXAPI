using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmployeeRealationMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_DependenceID")]
        public int NDependenceId { get; set; }
        [StringLength(100)]
        public string Dependent { get; set; }
        [StringLength(50)]
        public string Relation { get; set; }
        [Column("N_InsuranceID")]
        public int? NInsuranceId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
