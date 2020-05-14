using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwItemPos
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("X_PartNo")]
        [StringLength(250)]
        public string XPartNo { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_ClassName")]
        [StringLength(25)]
        public string XClassName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_WarehouseID")]
        public int? NWarehouseId { get; set; }
        [Column("I_Image", TypeName = "image")]
        public byte[] IImage { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("X_ItemName_a")]
        [StringLength(800)]
        public string XItemNameA { get; set; }
    }
}
