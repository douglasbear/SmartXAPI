using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sec_ModuleContents")]
    public partial class SecModuleContents
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ModuleContentID")]
        public int NModuleContentId { get; set; }
        [Column("N_ModuleID")]
        public int? NModuleId { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
    }
}
