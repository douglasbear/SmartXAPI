using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseOrderQty
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_POQty")]
        public double? NPoqty { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
