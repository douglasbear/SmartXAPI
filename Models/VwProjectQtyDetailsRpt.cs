using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectQtyDetailsRpt
    {
        [Column("MDQty")]
        public double? Mdqty { get; set; }
        [Column("DRQTY")]
        public double? Drqty { get; set; }
        [Column("PQTY")]
        public double? Pqty { get; set; }
        [Column("PTRQTY")]
        public double? Ptrqty { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Required]
        [StringLength(3)]
        public string TypeVal { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("N_ProjectId")]
        public int? NProjectId { get; set; }
    }
}
