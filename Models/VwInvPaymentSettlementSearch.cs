using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPaymentSettlementSearch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_SettlementID")]
        public int NSettlementId { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
        [StringLength(50)]
        public string Memo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("Vendor Code")]
        [StringLength(50)]
        public string VendorCode { get; set; }
        [Column("Vendor Name")]
        [StringLength(100)]
        public string VendorName { get; set; }
        [Column("D_VoucherDate", TypeName = "datetime")]
        public DateTime? DVoucherDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
    }
}
