using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Tvl_Ticketing")]
    public partial class TvlTicketing
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FnyearID")]
        public int NFnyearId { get; set; }
        [Key]
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
        [Column("D_RefundDate", TypeName = "date")]
        public DateTime? DRefundDate { get; set; }
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
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_NonRefundable")]
        public bool? BNonRefundable { get; set; }
        [Column("X_Description")]
        public string XDescription { get; set; }
        [Column("B_IsExist")]
        public bool? BIsExist { get; set; }
        [Column("N_CustRefund", TypeName = "money")]
        public decimal? NCustRefund { get; set; }
        [Column("N_SuppRefund", TypeName = "money")]
        public decimal? NSuppRefund { get; set; }
        [Column("N_RefundCustomerID")]
        public int? NRefundCustomerId { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("N_CustomerAmt", TypeName = "money")]
        public decimal? NCustomerAmt { get; set; }
        [Column("N_VendorAmt", TypeName = "money")]
        public decimal? NVendorAmt { get; set; }
    }
}
