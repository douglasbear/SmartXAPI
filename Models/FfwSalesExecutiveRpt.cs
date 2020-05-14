using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class FfwSalesExecutiveRpt
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("D_InvoiceDate", TypeName = "smalldatetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("N_InvoiceId")]
        public int NInvoiceId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("X_GrossWt", TypeName = "money")]
        public decimal? XGrossWt { get; set; }
        [Column("X_ChgWt", TypeName = "money")]
        public decimal? XChgWt { get; set; }
        [Column("N_Profit", TypeName = "money")]
        public decimal? NProfit { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(6)]
        public string XType { get; set; }
        [Column("N_BillAmount", TypeName = "money")]
        public decimal? NBillAmount { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_Month")]
        public int? NMonth { get; set; }
        [Column("X_MonthName")]
        [StringLength(30)]
        public string XMonthName { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_FileRefNo")]
        [StringLength(50)]
        public string XFileRefNo { get; set; }
    }
}
