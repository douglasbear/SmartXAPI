﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMultipleBranch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("B_DefaultBranch")]
        public bool? BDefaultBranch { get; set; }
    }
}
