using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesOrderQuantity
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Required]
        [StringLength(2)]
        public string Type { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
    }
}
