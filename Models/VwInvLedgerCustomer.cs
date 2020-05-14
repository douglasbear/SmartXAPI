using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvLedgerCustomer
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("D_SalesDate", TypeName = "datetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column(TypeName = "money")]
        public decimal? AmountDr { get; set; }
        [Column(TypeName = "money")]
        public decimal? AmountCr { get; set; }
        [StringLength(1000)]
        public string Notes { get; set; }
    }
}
