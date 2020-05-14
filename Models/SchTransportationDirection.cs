using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_TransportationDirection")]
    public partial class SchTransportationDirection
    {
        [Key]
        [Column("N_DirectionID")]
        public int NDirectionId { get; set; }
        [Column("X_Direction")]
        [StringLength(50)]
        public string XDirection { get; set; }
    }
}
