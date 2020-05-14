using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_ServiceEndType")]
    public partial class PayServiceEndType
    {
        [Key]
        [Column("N_EndTypeID")]
        public int NEndTypeId { get; set; }
        [Column("X_EndType")]
        [StringLength(50)]
        public string XEndType { get; set; }
    }
}
