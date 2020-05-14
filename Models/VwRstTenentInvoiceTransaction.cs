using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRstTenentInvoiceTransaction
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BatchID")]
        public int NBatchId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ProcessDate", TypeName = "smalldatetime")]
        public DateTime? DProcessDate { get; set; }
        [Column("N_Month")]
        public int? NMonth { get; set; }
        [Column("N_Year")]
        public int? NYear { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_InvoiceDetail")]
        public int NInvoiceDetail { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
    }
}
