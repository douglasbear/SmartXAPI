using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesBySalesOrder
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesOrderID")]
        public int? NSalesOrderId { get; set; }
        [Column("Sales_qty")]
        public double? SalesQty { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
    }
}
