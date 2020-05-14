using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class View4
    {
        [Column("N_LinkID")]
        public int NLinkId { get; set; }
        [Column("X_Subject")]
        public string XSubject { get; set; }
        [Column("X_Url")]
        public string XUrl { get; set; }
    }
}
