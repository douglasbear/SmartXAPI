using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesCost
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("sales_amt", TypeName = "money")]
        public decimal? SalesAmt { get; set; }
    }
}
