using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCustomerBalanceAdjustment
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("Customer Name")]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Column("N_PartyType")]
        public int? NPartyType { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("Adjustment Date")]
        [StringLength(8000)]
        public string AdjustmentDate { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("Net Amount")]
        [StringLength(30)]
        public string NetAmount { get; set; }
    }
}
