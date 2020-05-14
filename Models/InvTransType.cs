using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_TransType")]
    public partial class InvTransType
    {
        [Key]
        [Column("N_TransTypeID")]
        public int NTransTypeId { get; set; }
        [Column("X_TransName")]
        [StringLength(50)]
        public string XTransName { get; set; }
        [Column("N_TransType")]
        public int NTransType { get; set; }
        [Column("X_TransNameDisp")]
        [StringLength(50)]
        public string XTransNameDisp { get; set; }
    }
}
