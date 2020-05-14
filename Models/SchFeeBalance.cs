using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class SchFeeBalance
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column("ReceiptAMount", TypeName = "money")]
        public decimal ReceiptAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? Balance { get; set; }
    }
}
