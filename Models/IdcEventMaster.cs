using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("IDC_EventMaster")]
    public partial class IdcEventMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EventID")]
        public int? NEventId { get; set; }
        [Column("X_EventCode")]
        [StringLength(50)]
        public string XEventCode { get; set; }
        [Column("X_Event")]
        [StringLength(500)]
        public string XEvent { get; set; }
    }
}
