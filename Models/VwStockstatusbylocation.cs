using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStockstatusbylocation
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
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Required]
        [Column("X_Rack")]
        [StringLength(50)]
        public string XRack { get; set; }
        [Column("N_ItemUnitID")]
        public int NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_CurrentStock")]
        [StringLength(8000)]
        public string NCurrentStock { get; set; }
        [Column("N_CurrStock")]
        public double? NCurrStock { get; set; }
        [Column("N_MinQty")]
        public double? NMinQty { get; set; }
        [Column("N_ReOrderQty")]
        public double? NReOrderQty { get; set; }
        [Column("N_PreferredVendorID")]
        public int? NPreferredVendorId { get; set; }
        [Required]
        [StringLength(1)]
        public string TransDate { get; set; }
        [Column("SOQTY")]
        [StringLength(10)]
        public string Soqty { get; set; }
        [Column("X_PartNo")]
        [StringLength(250)]
        public string XPartNo { get; set; }
        [Column("X_ItemManufacturer")]
        [StringLength(50)]
        public string XItemManufacturer { get; set; }
        [Required]
        [Column("X_PreferredVendor")]
        [StringLength(100)]
        public string XPreferredVendor { get; set; }
    }
}
