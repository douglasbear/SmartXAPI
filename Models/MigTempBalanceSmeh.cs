using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_TempBalanceSMEH")]
    public partial class MigTempBalanceSmeh
    {
        [StringLength(200)]
        public string StoreNo { get; set; }
        [StringLength(200)]
        public string PartNo { get; set; }
        [Column("Al_hayer")]
        [StringLength(200)]
        public string AlHayer { get; set; }
        [Column("waadi laban")]
        [StringLength(200)]
        public string WaadiLaban { get; set; }
        [StringLength(200)]
        public string Office { get; set; }
        [Column("Temp_Old")]
        [StringLength(200)]
        public string TempOld { get; set; }
        [Column("Temp_2016")]
        [StringLength(200)]
        public string Temp2016 { get; set; }
        [Column("Temp_2017")]
        [StringLength(200)]
        public string Temp2017 { get; set; }
        [StringLength(200)]
        public string Total { get; set; }
        [Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Column(TypeName = "money")]
        public decimal? Cost { get; set; }
        [Column("Al_hayer_1")]
        [StringLength(200)]
        public string AlHayer1 { get; set; }
        [Column("waadi laban_1")]
        [StringLength(200)]
        public string WaadiLaban1 { get; set; }
        [Column("Office_1")]
        [StringLength(200)]
        public string Office1 { get; set; }
        [Column("Temp_Old1")]
        [StringLength(200)]
        public string TempOld1 { get; set; }
        [Column("Temp_2016_1")]
        [StringLength(200)]
        public string Temp20161 { get; set; }
        [Column("Temp_2017_1")]
        [StringLength(200)]
        public string Temp20171 { get; set; }
        [StringLength(200)]
        public string Vendor { get; set; }
        [StringLength(200)]
        public string Category { get; set; }
        [Column("UOM")]
        [StringLength(200)]
        public string Uom { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
    }
}
