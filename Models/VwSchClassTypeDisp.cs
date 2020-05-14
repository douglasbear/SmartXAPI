using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClassTypeDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [StringLength(30)]
        public string Code { get; set; }
        [StringLength(50)]
        public string Section { get; set; }
    }
}
