using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTvlTicketing
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int NFnyearId { get; set; }
        [Column("N_TicketID")]
        public int NTicketId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_TicketDate", TypeName = "datetime")]
        public DateTime? DTicketDate { get; set; }
        [Column("X_TicketNo")]
        [StringLength(50)]
        public string XTicketNo { get; set; }
        [Column("X_Passenger")]
        [StringLength(100)]
        public string XPassenger { get; set; }
        [Column("X_Route")]
        [StringLength(100)]
        public string XRoute { get; set; }
        [Column("N_TicketTypeID")]
        public int? NTicketTypeId { get; set; }
        [Column("D_TravelDate", TypeName = "datetime")]
        public DateTime? DTravelDate { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_SuppFare", TypeName = "money")]
        public decimal? NSuppFare { get; set; }
        [Column("N_SuppTax", TypeName = "money")]
        public decimal? NSuppTax { get; set; }
        [Column("N_Commission", TypeName = "money")]
        public decimal? NCommission { get; set; }
        [Column("N_SuppCommission", TypeName = "money")]
        public decimal? NSuppCommission { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_CustFare", TypeName = "money")]
        public decimal? NCustFare { get; set; }
        [Column("N_CustTax", TypeName = "money")]
        public decimal? NCustTax { get; set; }
        [Column("N_ServiceCharge", TypeName = "money")]
        public decimal NServiceCharge { get; set; }
        [Required]
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Column("N_UsedFare", TypeName = "money")]
        public decimal? NUsedFare { get; set; }
        [Column("N_SuppPenalty", TypeName = "money")]
        public decimal? NSuppPenalty { get; set; }
        [Column("N_CustPenalty", TypeName = "money")]
        public decimal? NCustPenalty { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_TicketType")]
        [StringLength(50)]
        public string XTicketType { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("Ticket Date")]
        [StringLength(8000)]
        public string TicketDate { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("B_NonRefundable")]
        public bool? BNonRefundable { get; set; }
        [Column("X_Description")]
        public string XDescription { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("B_IsExist")]
        public bool? BIsExist { get; set; }
        [Column("D_RefundDate", TypeName = "date")]
        public DateTime? DRefundDate { get; set; }
        [Required]
        [StringLength(6)]
        public string Status { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        public int? VendorType { get; set; }
        [Column("N_CustRefund", TypeName = "money")]
        public decimal? NCustRefund { get; set; }
        [Column("N_SuppRefund", TypeName = "money")]
        public decimal? NSuppRefund { get; set; }
        [Column("N_RefundCustomerID")]
        public int? NRefundCustomerId { get; set; }
        [Column("X_RefundCustomerCode")]
        [StringLength(50)]
        public string XRefundCustomerCode { get; set; }
        [Column("X_RefundCustomerName")]
        [StringLength(100)]
        public string XRefundCustomerName { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("X_SalesmanCode")]
        [StringLength(50)]
        public string XSalesmanCode { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("N_VendorAmt", TypeName = "money")]
        public decimal? NVendorAmt { get; set; }
        [Column("N_CustomerAmt", TypeName = "money")]
        public decimal? NCustomerAmt { get; set; }
        public double? CustServiceCharge { get; set; }
        [Column("N_VendorTotal")]
        [StringLength(30)]
        public string NVendorTotal { get; set; }
        [Column("N_CustomerTotal")]
        [StringLength(30)]
        public string NCustomerTotal { get; set; }
        [StringLength(100)]
        public string Customer { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
    }
}
