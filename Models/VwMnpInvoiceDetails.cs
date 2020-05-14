using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMnpInvoiceDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_InvoiceDetailsID")]
        public int NInvoiceDetailsId { get; set; }
        [Column("N_MobilizationID")]
        public int NMobilizationId { get; set; }
        [Column("N_MaintenanceID")]
        public int NMaintenanceId { get; set; }
        [Column("N_MobilizationDetailsID")]
        public int NMobilizationDetailsId { get; set; }
        [Column("N_Days")]
        public int NDays { get; set; }
        [Column("N_PayRate", TypeName = "money")]
        public decimal NPayRate { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal NDiscount { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Required]
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime DInvoiceDate { get; set; }
        [Column("X_Month")]
        [StringLength(50)]
        public string XMonth { get; set; }
        [Column("X_Year")]
        [StringLength(50)]
        public string XYear { get; set; }
        [Column("N_PayRun")]
        public int? NPayRun { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_EmployeeCode")]
        [StringLength(400)]
        public string XEmployeeCode { get; set; }
        [Column("X_EmployeeName")]
        [StringLength(400)]
        public string XEmployeeName { get; set; }
        [Required]
        [Column("X_MobilizationCode")]
        [StringLength(20)]
        public string XMobilizationCode { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_ContactNo")]
        [StringLength(20)]
        public string XContactNo { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_HiredRate", TypeName = "money")]
        public decimal? NHiredRate { get; set; }
        [Column("X_OrderNo")]
        [StringLength(10)]
        public string XOrderNo { get; set; }
        [Column("X_MaintenanceCode")]
        [StringLength(400)]
        public string XMaintenanceCode { get; set; }
    }
}
