using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwIdcPrint
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CardID")]
        public int NCardId { get; set; }
        [Column("X_CardCode")]
        [StringLength(50)]
        public string XCardCode { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_Place")]
        [StringLength(50)]
        public string XPlace { get; set; }
        [Column("D_DOB", TypeName = "smalldatetime")]
        public DateTime? DDob { get; set; }
        [Column(TypeName = "image")]
        public byte[] Image { get; set; }
    }
}
