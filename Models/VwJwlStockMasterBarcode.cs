using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlStockMasterBarcode
    {
        [Column("X_Model")]
        [StringLength(50)]
        public string XModel { get; set; }
    }
}
