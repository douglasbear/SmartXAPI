using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class Name
    {
        [Column("ID")]
        public int? Id { get; set; }
        [Column("Name")]
        [StringLength(50)]
        public string Name1 { get; set; }
    }
}
