using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvCustomerMobilizationDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [Column("Customer Name")]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [Required]
        [Column("Contact Person")]
        [StringLength(100)]
        public string ContactPerson { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [Column("Zip Code")]
        [StringLength(25)]
        public string ZipCode { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [Column("Phone No1")]
        [StringLength(20)]
        public string PhoneNo1 { get; set; }
        [Column("Phone No2")]
        [StringLength(20)]
        public string PhoneNo2 { get; set; }
        [Column("Fax No")]
        [StringLength(100)]
        public string FaxNo { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(50)]
        public string Website { get; set; }
        [Column("N_CreditLimit", TypeName = "money")]
        public decimal? NCreditLimit { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_DefaultSalesManID")]
        public int NDefaultSalesManId { get; set; }
        [Column("B_DirPosting")]
        public bool? BDirPosting { get; set; }
        [Required]
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_EnablePopup")]
        public bool? NEnablePopup { get; set; }
        [Column("N_CashTypeID")]
        public int? NCashTypeId { get; set; }
    }
}
