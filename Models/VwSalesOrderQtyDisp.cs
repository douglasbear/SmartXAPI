using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesOrderQtyDisp
    {
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
    }
}
