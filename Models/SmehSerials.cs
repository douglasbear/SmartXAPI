using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__SMEH_Serials")]
    public partial class SmehSerials
    {
        [Column("Store_No")]
        [StringLength(200)]
        public string StoreNo { get; set; }
        [Column("Part_No")]
        [StringLength(300)]
        public string PartNo { get; set; }
        [StringLength(300)]
        public string Srl1 { get; set; }
        [StringLength(300)]
        public string Srl2 { get; set; }
        [StringLength(300)]
        public string Srl3 { get; set; }
        [StringLength(300)]
        public string Srl4 { get; set; }
        [StringLength(300)]
        public string Srl5 { get; set; }
        [StringLength(300)]
        public string Srl6 { get; set; }
        [StringLength(300)]
        public string Srl7 { get; set; }
        [StringLength(300)]
        public string Srl8 { get; set; }
        [StringLength(300)]
        public string Srl9 { get; set; }
        [StringLength(300)]
        public string Srl10 { get; set; }
        [StringLength(300)]
        public string Srl11 { get; set; }
        [StringLength(300)]
        public string Srl12 { get; set; }
        [StringLength(300)]
        public string Srl13 { get; set; }
        [StringLength(300)]
        public string Srl14 { get; set; }
        [StringLength(300)]
        public string Srl15 { get; set; }
        [StringLength(300)]
        public string Srl16 { get; set; }
        [StringLength(300)]
        public string Srl17 { get; set; }
        [StringLength(300)]
        public string Srl18 { get; set; }
        [StringLength(300)]
        public string Srl19 { get; set; }
        [StringLength(300)]
        public string Srl20 { get; set; }
        [StringLength(300)]
        public string Srl21 { get; set; }
        [StringLength(300)]
        public string Srl22 { get; set; }
        [StringLength(300)]
        public string Srl23 { get; set; }
        [StringLength(300)]
        public string Srl24 { get; set; }
        [StringLength(300)]
        public string Srl25 { get; set; }
        [StringLength(300)]
        public string Location { get; set; }
        [StringLength(100)]
        public string Total { get; set; }
        [StringLength(100)]
        public string Vendor { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [Column("UOM")]
        [StringLength(100)]
        public string Uom { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
    }
}
