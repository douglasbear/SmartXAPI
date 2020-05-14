using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesmanDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_SalesmanID")]
        public int NSalesmanId { get; set; }
        [Column("Salesman Code")]
        [StringLength(50)]
        public string SalesmanCode { get; set; }
        [Column("Salesman Name")]
        [StringLength(100)]
        public string SalesmanName { get; set; }
        [Column("Contact Name")]
        [StringLength(100)]
        public string ContactName { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [Column("Zip Code")]
        [StringLength(25)]
        public string ZipCode { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [Column("Phone No")]
        [StringLength(50)]
        public string PhoneNo { get; set; }
        [Column("Phone No2")]
        [StringLength(20)]
        public string PhoneNo2 { get; set; }
        [Column("Fax No")]
        [StringLength(100)]
        public string FaxNo { get; set; }
        [Column("E-Mail")]
        [StringLength(50)]
        public string EMail { get; set; }
        [Column("Web site")]
        [StringLength(50)]
        public string WebSite { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_CreditLimit", TypeName = "money")]
        public decimal? NCreditLimit { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [StringLength(50)]
        public string Commission { get; set; }
        [Column("N_CommnPerc")]
        public double? NCommnPerc { get; set; }
    }
}
