using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaySummaryForHoursCalc
    {
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_SummaryValue", TypeName = "money")]
        public decimal? NSummaryValue { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
