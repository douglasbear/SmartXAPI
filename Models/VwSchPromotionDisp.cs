using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchPromotionDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("Process No")]
        public int ProcessNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        [Column("From Section")]
        [StringLength(50)]
        public string FromSection { get; set; }
        [Column("From Class")]
        [StringLength(50)]
        public string FromClass { get; set; }
        [Column("To Section")]
        [StringLength(50)]
        public string ToSection { get; set; }
        [Column("To Class")]
        [StringLength(50)]
        public string ToClass { get; set; }
    }
}
