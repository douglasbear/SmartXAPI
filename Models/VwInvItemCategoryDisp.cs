using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemCategoryDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        public int Code { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [StringLength(30)]
        public string CategoryCode { get; set; }
    }
}
