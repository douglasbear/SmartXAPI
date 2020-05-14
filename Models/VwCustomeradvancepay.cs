using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCustomeradvancepay
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("N_SaleAmount", TypeName = "money")]
        public decimal? NSaleAmount { get; set; }
        [Column("BranchID")]
        public int? BranchId { get; set; }
        [Column("N_advanceamount", TypeName = "money")]
        public decimal? NAdvanceamount { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Required]
        [StringLength(9)]
        public string Status { get; set; }
    }
}
