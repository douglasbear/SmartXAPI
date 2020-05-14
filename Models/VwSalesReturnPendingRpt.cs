using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesReturnPendingRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_SalesOrderID")]
        public int? NSalesOrderId { get; set; }
    }
}
