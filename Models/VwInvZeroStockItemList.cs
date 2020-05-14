using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvZeroStockItemList
    {
        [Column("D_TransferDate", TypeName = "smalldatetime")]
        public DateTime? DTransferDate { get; set; }
        [Column("Item Code")]
        [StringLength(50)]
        public string ItemCode { get; set; }
        [Column("Item Name")]
        [StringLength(600)]
        public string ItemName { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [Column("Rack/Location")]
        [StringLength(50)]
        public string RackLocation { get; set; }
        [Column("Current Stock")]
        public double? CurrentStock { get; set; }
        [Column("U/M")]
        [StringLength(500)]
        public string UM { get; set; }
    }
}
