using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTransactionType
    {
        [Required]
        [Column("X_ID")]
        [StringLength(20)]
        public string XId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
    }
}
