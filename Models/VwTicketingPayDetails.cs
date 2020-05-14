using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTicketingPayDetails
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_TicketID")]
        public int NTicketId { get; set; }
        [Column("X_TicketNo")]
        [StringLength(50)]
        public string XTicketNo { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Required]
        [StringLength(2)]
        public string TransType { get; set; }
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
    }
}
