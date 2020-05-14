using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccTransType
    {
        [Column("X_EntryForm")]
        [StringLength(50)]
        public string XEntryForm { get; set; }
    }
}
