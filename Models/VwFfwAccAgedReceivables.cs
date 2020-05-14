using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwAccAgedReceivables
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int? NFnyearId { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount1 { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount2 { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount3 { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount4 { get; set; }
    }
}
