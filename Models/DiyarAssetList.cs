using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__DiyarAssetList")]
    public partial class DiyarAssetList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [StringLength(200)]
        public string Category { get; set; }
        [Column("Asset_Code")]
        [StringLength(300)]
        public string AssetCode { get; set; }
        [StringLength(300)]
        public string Asset { get; set; }
        [StringLength(10)]
        public string Qty { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(100)]
        public string Location { get; set; }
        [Column("Cost_Center")]
        [StringLength(100)]
        public string CostCenter { get; set; }
        [Column("Aquisition_Date", TypeName = "date")]
        public DateTime? AquisitionDate { get; set; }
        [Column("Life_Year")]
        [StringLength(100)]
        public string LifeYear { get; set; }
        [Column("Purchase_Cost")]
        [StringLength(100)]
        public string PurchaseCost { get; set; }
        [StringLength(100)]
        public string Depreciation { get; set; }
        [Column("Net_BookValue")]
        [StringLength(100)]
        public string NetBookValue { get; set; }
    }
}
