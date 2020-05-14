using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwUserRoleDisp
    {
        [StringLength(50)]
        public string Category { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Column("N_UserCategoryID")]
        public int NUserCategoryId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
