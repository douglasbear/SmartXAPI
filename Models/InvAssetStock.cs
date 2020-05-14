using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_AssetStock")]
    public partial class InvAssetStock
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AssetStockID")]
        public int? NAssetStockId { get; set; }
        [Column("N_")]
        [StringLength(10)]
        public string N { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
