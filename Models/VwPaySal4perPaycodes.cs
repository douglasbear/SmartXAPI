using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaySal4perPaycodes
    {
        [Column("N_percPayID")]
        public int? NPercPayId { get; set; }
        [Column("N_FixedPayID")]
        public int NFixedPayId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }
    }
}
