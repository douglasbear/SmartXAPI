using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMnpInvoiceMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Required]
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime DInvoiceDate { get; set; }
        [Column("X_Month")]
        [StringLength(50)]
        public string XMonth { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("X_Year")]
        [StringLength(50)]
        public string XYear { get; set; }
        [Column("N_PayRun")]
        public int? NPayRun { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("D_InvoiceFromDate", TypeName = "datetime")]
        public DateTime DInvoiceFromDate { get; set; }
        [Column("D_InvoiceToDate", TypeName = "datetime")]
        public DateTime DInvoiceToDate { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal NTotalAmount { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal NTaxAmount { get; set; }
        [Column("N_NetAmount", TypeName = "money")]
        public decimal NNetAmount { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_Days")]
        public double? NDays { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_PayFactor")]
        public double? NPayFactor { get; set; }
        [Column("X_GroupName")]
        [StringLength(50)]
        public string XGroupName { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
    }
}
