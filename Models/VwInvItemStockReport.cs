using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemStockReport
    {
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Required]
        [StringLength(3)]
        public string Type { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TranDate { get; set; }
        [StringLength(50)]
        public string TranNo { get; set; }
        [StringLength(100)]
        public string TranByName { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
