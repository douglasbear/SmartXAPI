using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VsaCustomerRcptDetail
    {
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(10)]
        public string XType { get; set; }
        [Column("AMount", TypeName = "money")]
        public decimal? Amount { get; set; }
    }
}
