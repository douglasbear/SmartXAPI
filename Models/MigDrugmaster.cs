using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_Drugmaster")]
    public partial class MigDrugmaster
    {
        [Column("Drug_id")]
        public int? DrugId { get; set; }
        [Column("Drug_Name")]
        [StringLength(200)]
        public string DrugName { get; set; }
        [Column("Means_Qty")]
        [StringLength(200)]
        public string MeansQty { get; set; }
        [Column("Means_Type")]
        [StringLength(200)]
        public string MeansType { get; set; }
        [Column("Drug_Type")]
        [StringLength(200)]
        public string DrugType { get; set; }
        [Column("Qty_Unit")]
        [StringLength(200)]
        public string QtyUnit { get; set; }
        [Column("Maf_id")]
        [StringLength(200)]
        public string MafId { get; set; }
        [StringLength(200)]
        public string Comments { get; set; }
        [StringLength(200)]
        public string Activate { get; set; }
        [Column("Drug_Selection")]
        [StringLength(200)]
        public string DrugSelection { get; set; }
        [StringLength(200)]
        public string Category { get; set; }
        [Column("Generic_Name")]
        [StringLength(200)]
        public string GenericName { get; set; }
        [StringLength(200)]
        public string Control { get; set; }
        [Column("Bin_num")]
        [StringLength(200)]
        public string BinNum { get; set; }
        [Column("Item_Code")]
        [StringLength(200)]
        public string ItemCode { get; set; }
        [Column("G_name")]
        [StringLength(200)]
        public string GName { get; set; }
        [Column("Brand Name")]
        [StringLength(200)]
        public string BrandName { get; set; }
        [Column("Max_Discount")]
        [StringLength(200)]
        public string MaxDiscount { get; set; }
        [StringLength(200)]
        public string Expiry { get; set; }
        [Column("LineID")]
        [StringLength(200)]
        public string LineId { get; set; }
        [Column("Date_Cre")]
        [StringLength(200)]
        public string DateCre { get; set; }
        [Column("user")]
        [StringLength(200)]
        public string User { get; set; }
        [Column("SP")]
        [StringLength(200)]
        public string Sp { get; set; }
        [StringLength(200)]
        public string Unitcost { get; set; }
        [Column("SP2")]
        [StringLength(200)]
        public string Sp2 { get; set; }
        [Column("SP3")]
        [StringLength(200)]
        public string Sp3 { get; set; }
    }
}
