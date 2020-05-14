using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccPeriod
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PeriodID")]
        public int NPeriodId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("Start Date")]
        [StringLength(8000)]
        public string StartDate { get; set; }
        [Column("End Date")]
        [StringLength(8000)]
        public string EndDate { get; set; }
        [Column("PeriodID")]
        [StringLength(50)]
        public string PeriodId { get; set; }
    }
}
