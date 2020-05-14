using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalaryUpdateMonth
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        public int MonthNmbr { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
    }
}
