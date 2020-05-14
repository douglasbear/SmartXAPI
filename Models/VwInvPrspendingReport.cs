using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPrspendingReport
    {
        [Column("D_PRSDate", TypeName = "datetime")]
        public DateTime? DPrsdate { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("Qty_Order")]
        public double? QtyOrder { get; set; }
        [Column("Qty_Deliver")]
        public double QtyDeliver { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [StringLength(500)]
        public string Unit { get; set; }
        [Column("X_TransName")]
        [StringLength(50)]
        public string XTransName { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
    }
}
