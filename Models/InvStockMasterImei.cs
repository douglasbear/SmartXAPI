using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockMaster_IMEI")]
    public partial class InvStockMasterImei
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_Stock_IMEIID")]
        public int NStockImeiid { get; set; }
        [Column("N_StockID")]
        public int? NStockId { get; set; }
        [Required]
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_Comments")]
        [StringLength(250)]
        public string XComments { get; set; }
    }
}
