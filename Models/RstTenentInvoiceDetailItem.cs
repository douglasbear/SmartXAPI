using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rst_TenentInvoiceDetailItem")]
    public partial class RstTenentInvoiceDetailItem
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Key]
        [Column("N_InvoiceDetail")]
        public int NInvoiceDetail { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_batchID")]
        public int? NBatchId { get; set; }
        [Column("X_ExtensionCode")]
        [StringLength(10)]
        public string XExtensionCode { get; set; }
    }
}
