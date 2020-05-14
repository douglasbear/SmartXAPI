using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvCreditNoteNoSearch
    {
        [Column("Credit Note No")]
        [StringLength(50)]
        public string CreditNoteNo { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? Date { get; set; }
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Column("Invoice Date", TypeName = "smalldatetime")]
        public DateTime? InvoiceDate { get; set; }
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [StringLength(100)]
        public string Customer { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
    }
}
