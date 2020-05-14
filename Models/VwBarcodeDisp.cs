using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBarcodeDisp
    {
        [Required]
        [StringLength(8)]
        public string Type { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("D_TransDate")]
        [StringLength(15)]
        public string DTransDate { get; set; }
        [Column("X_TransactionNo")]
        [StringLength(50)]
        public string XTransactionNo { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("B_BarcodeSalePrint")]
        public bool? BBarcodeSalePrint { get; set; }
        [Column("B_BarcodePurPrint")]
        public bool? BBarcodePurPrint { get; set; }
    }
}
