using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjPaymentsReceipts
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseId")]
        public int NPurchaseId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_ReferenseNo")]
        [StringLength(66)]
        public string XReferenseNo { get; set; }
        [Column(TypeName = "money")]
        public decimal? PayableAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? ReceivableAmount { get; set; }
        [Column("N_ReturnAmount")]
        public int NReturnAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? Paid { get; set; }
        [Column(TypeName = "money")]
        public decimal? Received { get; set; }
        [Column(TypeName = "money")]
        public decimal? BalancePayable { get; set; }
        [Column(TypeName = "money")]
        public decimal? BalanceReceivable { get; set; }
        [Column("ProjectID")]
        public int? ProjectId { get; set; }
        public int? TransType { get; set; }
        [Required]
        [StringLength(6)]
        public string Vendor { get; set; }
        [Required]
        [StringLength(5)]
        public string Agent { get; set; }
        [Required]
        [StringLength(6)]
        public string Client { get; set; }
        [StringLength(50)]
        public string PartyCode { get; set; }
        [StringLength(100)]
        public string PartyName { get; set; }
    }
}
