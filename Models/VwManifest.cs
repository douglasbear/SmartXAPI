using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwManifest
    {
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("desc1", TypeName = "money")]
        public decimal? Desc1 { get; set; }
        [Column("desc2", TypeName = "money")]
        public decimal? Desc2 { get; set; }
        [Column("B_Prepayment")]
        public bool BPrepayment { get; set; }
        [Required]
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
        [Required]
        [Column("X_Driver")]
        [StringLength(100)]
        public string XDriver { get; set; }
    }
}
