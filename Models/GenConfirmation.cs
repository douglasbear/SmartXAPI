using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_Confirmation")]
    public partial class GenConfirmation
    {
        [Column("X_ConfirmStatus")]
        [StringLength(50)]
        public string XConfirmStatus { get; set; }
        [Column("X_ConfirmName")]
        [StringLength(50)]
        public string XConfirmName { get; set; }
        [Column("N_ConfirmID")]
        public int? NConfirmId { get; set; }
    }
}
