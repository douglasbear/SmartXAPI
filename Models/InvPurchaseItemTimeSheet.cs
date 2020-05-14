using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class InvPurchaseItemTimeSheet
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("D_Invoicedate", TypeName = "datetime")]
        public DateTime? DInvoicedate { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
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
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_Hour")]
        public double? NHour { get; set; }
        [Column("X_Month")]
        [StringLength(30)]
        public string XMonth { get; set; }
        [Column("N_ServiceSheetDetailsID")]
        public int NServiceSheetDetailsId { get; set; }
        [Column("X_ServiceSheetCode")]
        [StringLength(100)]
        public string XServiceSheetCode { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
    }
}
