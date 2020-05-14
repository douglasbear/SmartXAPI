using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSoReturnDetails
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("N_SalesOrderDetailsID")]
        public int NSalesOrderDetailsId { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
    }
}
