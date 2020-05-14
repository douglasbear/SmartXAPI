using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPopendingReport
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("D_POrderDate", TypeName = "datetime")]
        public DateTime? DPorderDate { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("B_CancelOrder")]
        public bool? BCancelOrder { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("D_ExDelvDate", TypeName = "datetime")]
        public DateTime? DExDelvDate { get; set; }
        [Column("N_QtyToStock")]
        public int? NQtyToStock { get; set; }
        [StringLength(500)]
        public string Unit { get; set; }
    }
}
