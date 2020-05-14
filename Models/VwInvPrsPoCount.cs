using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPrsPoCount
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_ProcessedQty")]
        public double? NProcessedQty { get; set; }
        [Column("N_SalesOrderDetailsID")]
        public int? NSalesOrderDetailsId { get; set; }
        [Column("N_TransTypeID")]
        public int NTransTypeId { get; set; }
    }
}
