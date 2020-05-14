using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayManagerEmailInformation
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_ManagerEmpId")]
        public int NManagerEmpId { get; set; }
        [Column("X_ManagerCode")]
        [StringLength(50)]
        public string XManagerCode { get; set; }
        [Column("X_ManagerEmpName")]
        [StringLength(100)]
        public string XManagerEmpName { get; set; }
        [Column("X_ManagerEmailID")]
        [StringLength(50)]
        public string XManagerEmailId { get; set; }
    }
}
