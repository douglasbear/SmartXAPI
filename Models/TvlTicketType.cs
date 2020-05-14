using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Tvl_TicketType")]
    public partial class TvlTicketType
    {
        [Column("N_TiketTypeID")]
        public int? NTiketTypeId { get; set; }
        [Column("X_TicketType")]
        [StringLength(50)]
        public string XTicketType { get; set; }
    }
}
