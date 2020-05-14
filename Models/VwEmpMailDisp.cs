using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpMailDisp
    {
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmailID")]
        [StringLength(50)]
        public string XEmailId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
