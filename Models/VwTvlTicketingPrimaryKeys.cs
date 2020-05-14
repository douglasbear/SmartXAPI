using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTvlTicketingPrimaryKeys
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int NFnyearId { get; set; }
        [Column("N_TicketID")]
        public int NTicketId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_TicketDate", TypeName = "datetime")]
        public DateTime? DTicketDate { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("N_CreditNoteId")]
        public int? NCreditNoteId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_DebitNoteId")]
        public int? NDebitNoteId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
    }
}
