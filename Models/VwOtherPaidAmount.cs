using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwOtherPaidAmount
    {
        [Column(TypeName = "money")]
        public decimal? OtherPaidAmount { get; set; }
        [Column("SalesID")]
        public int SalesId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
    }
}
