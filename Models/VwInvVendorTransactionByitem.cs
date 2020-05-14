using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvVendorTransactionByitem
    {
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int NPurchaseDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        [StringLength(30)]
        public string NQty { get; set; }
        [Column("N_PPrice")]
        [StringLength(30)]
        public string NPprice { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("D_InvoiceDate")]
        [StringLength(8000)]
        public string DInvoiceDate { get; set; }
        [Column("N_CompanyID")]
        [StringLength(30)]
        public string NCompanyId { get; set; }
        [StringLength(30)]
        public string TotalAmount { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_BaseUnitQty")]
        public double? NBaseUnitQty { get; set; }
        [Column("B_BaseUnit")]
        public bool? BBaseUnit { get; set; }
        [Column("N_PurchaseUnitID")]
        public int? NPurchaseUnitId { get; set; }
    }
}
