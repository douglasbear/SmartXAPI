using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesStockImei
    {
        [Column("N_StockID")]
        public int? NStockId { get; set; }
        [Column("IMEI")]
        [StringLength(30)]
        public string Imei { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
    }
}
