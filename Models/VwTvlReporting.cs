using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTvlReporting
    {
        [Column("D_TicketDate", TypeName = "datetime")]
        public DateTime? DTicketDate { get; set; }
        [Column("N_TicketID")]
        public int NTicketId { get; set; }
        [Column("X_ContactName")]
        [StringLength(100)]
        public string XContactName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("customer_amount", TypeName = "money")]
        public decimal? CustomerAmount { get; set; }
        [Column("purchase_amount", TypeName = "money")]
        public decimal? PurchaseAmount { get; set; }
        [Required]
        [Column("type1")]
        [StringLength(6)]
        public string Type1 { get; set; }
        [Column("X_TicketNo")]
        [StringLength(50)]
        public string XTicketNo { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("N_FnyearID")]
        public int NFnyearId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
    }
}
