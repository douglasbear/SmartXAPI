using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseOrder
    {
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [StringLength(50)]
        public string OrderNo { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_POrderDate", TypeName = "datetime")]
        public DateTime? DPorderDate { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("B_CancelOrder")]
        public bool? BCancelOrder { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_DeliveryPlaceID")]
        public int? NDeliveryPlaceId { get; set; }
        [Column("X_DeliveryPlace")]
        [StringLength(100)]
        public string XDeliveryPlace { get; set; }
        [Column("X_Description")]
        public string XDescription { get; set; }
        [Column("PO")]
        public double? Po { get; set; }
        [Column("MRNQTY")]
        public double? Mrnqty { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_POTypeID")]
        public int? NPotypeId { get; set; }
    }
}
