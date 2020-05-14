using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBranchMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("Branch Code")]
        [StringLength(50)]
        public string BranchCode { get; set; }
        [Column("Branch Name")]
        [StringLength(50)]
        public string BranchName { get; set; }
    }
}
