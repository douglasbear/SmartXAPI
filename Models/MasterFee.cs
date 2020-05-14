using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_MasterFee")]
    public partial class MasterFee
    {
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Column("X_FeeName")]
        [StringLength(200)]
        public string XFeeName { get; set; }
        [Column("N_TypeID")]
        [StringLength(300)]
        public string NTypeId { get; set; }
    }
}
