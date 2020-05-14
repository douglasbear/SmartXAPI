using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchTerm
    {
        [Column("X_Description")]
        [StringLength(25)]
        public string XDescription { get; set; }
    }
}
