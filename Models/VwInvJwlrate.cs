using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvJwlrate
    {
        [StringLength(100)]
        public string Name { get; set; }
        [Column(TypeName = "money")]
        public decimal? Rate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_RateID")]
        public int NRateId { get; set; }
    }
}
