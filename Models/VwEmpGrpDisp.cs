using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpGrpDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(50)]
        public string XGroupName { get; set; }
    }
}
