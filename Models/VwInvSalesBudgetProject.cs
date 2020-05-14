using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesBudgetProject
    {
        [Column("X_ProjectName")]
        [StringLength(1000)]
        public string XProjectName { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
    }
}
