using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesOrderQty
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_WarehouseID")]
        public int? NWarehouseId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_OrderQty")]
        public double? NOrderQty { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
