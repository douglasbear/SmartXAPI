using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrndepartmentWiseReport
    {
        [Column("D_MRNDate", TypeName = "datetime")]
        public DateTime? DMrndate { get; set; }
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Column("PRSNO")]
        [StringLength(50)]
        public string Prsno { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_QtyToStock")]
        public int? NQtyToStock { get; set; }
        [Column("N_ReturnQty")]
        public int? NReturnQty { get; set; }
        [Column("X_Reason")]
        [StringLength(50)]
        public string XReason { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_VendorInvoice")]
        [StringLength(15)]
        public string XVendorInvoice { get; set; }
        [Column("X_VendorDeliveryNote")]
        [StringLength(15)]
        public string XVendorDeliveryNote { get; set; }
        [Column("X_Description")]
        public string XDescription { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
    }
}
