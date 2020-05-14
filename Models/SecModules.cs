using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_Modules")]
    public partial class SecModules
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ModuleID")]
        public int NModuleId { get; set; }
        [Column("X_ModuleCode")]
        [StringLength(50)]
        public string XModuleCode { get; set; }
        [Column("X_ModuleName")]
        [StringLength(100)]
        public string XModuleName { get; set; }
        [Column("B_Integrated")]
        public bool? BIntegrated { get; set; }
    }
}
