using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwInvoiceDashboard
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_InvoiceId")]
        public int NInvoiceId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("X_FileRefNo")]
        [StringLength(50)]
        public string XFileRefNo { get; set; }
        [Column("X_AwbNo")]
        [StringLength(50)]
        public string XAwbNo { get; set; }
        [Column("D_InvoiceDate")]
        [StringLength(8000)]
        public string DInvoiceDate { get; set; }
        [Column("N_BillAmt")]
        [StringLength(30)]
        public string NBillAmt { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BillAmount", TypeName = "money")]
        public decimal? NBillAmount { get; set; }
        [Column("X_Status")]
        [StringLength(20)]
        public string XStatus { get; set; }
        [StringLength(30)]
        public string AttachmentCount { get; set; }
    }
}
