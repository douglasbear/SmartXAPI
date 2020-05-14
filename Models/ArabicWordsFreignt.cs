using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class ArabicWordsFreignt
    {
        [Column("N_InvoiceId")]
        public int NInvoiceId { get; set; }
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [StringLength(1000)]
        public string AmountInWords { get; set; }
    }
}
