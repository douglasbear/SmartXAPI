using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccPeriodDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PeriodID")]
        public int NPeriodId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("Start Date ")]
        [StringLength(50)]
        public string StartDate { get; set; }
        [Column("End Date")]
        [StringLength(50)]
        public string EndDate { get; set; }
        [Column("X_Period")]
        [StringLength(100)]
        public string XPeriod { get; set; }
        [Column("Period Code")]
        [StringLength(50)]
        public string PeriodCode { get; set; }
    }
}
