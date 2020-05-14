using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwSalesReceipt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_InvoiceId")]
        public int NInvoiceId { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(20)]
        public string XType { get; set; }
    }
}
