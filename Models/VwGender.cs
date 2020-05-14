using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGender
    {
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(6)]
        public string Gender { get; set; }
    }
}
