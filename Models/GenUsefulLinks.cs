using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_UsefulLinks")]
    public partial class GenUsefulLinks
    {
        [Key]
        [Column("N_LinkID")]
        public int NLinkId { get; set; }
        [Column("X_Subject")]
        public string XSubject { get; set; }
        [Column("X_Url")]
        public string XUrl { get; set; }
    }
}
