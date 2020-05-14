using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class InvSalesItemTimeSheet
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_Invoicedate", TypeName = "datetime")]
        public DateTime? DInvoicedate { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("Total_Hours")]
        public double? TotalHours { get; set; }
        [Column("X_ServiceSheetCode")]
        [StringLength(100)]
        public string XServiceSheetCode { get; set; }
        [Column("N_Hour")]
        public double? NHour { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_Month")]
        [StringLength(30)]
        public string XMonth { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        [Column("N_ServiceSheetDetailsID")]
        public int NServiceSheetDetailsId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
    }
}
