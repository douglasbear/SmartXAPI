using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTvlPaymentDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int NFnyearId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [StringLength(250)]
        public string CustomerAddress { get; set; }
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [StringLength(250)]
        public string VendorAddress { get; set; }
        [Column("N_TicketID")]
        public int NTicketId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("X_TicketNo")]
        [StringLength(50)]
        public string XTicketNo { get; set; }
        [Column(TypeName = "money")]
        public decimal? ReceivedAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? PaidAmount { get; set; }
        [Column("D_RefundDate", TypeName = "date")]
        public DateTime? DRefundDate { get; set; }
    }
}
