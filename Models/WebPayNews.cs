using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Web_Pay_News")]
    public partial class WebPayNews
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_NewsID")]
        public int NNewsId { get; set; }
        [Column("X_NewsTitle")]
        [StringLength(100)]
        public string XNewsTitle { get; set; }
        [Column("X_NewsDesc")]
        [StringLength(500)]
        public string XNewsDesc { get; set; }
        [Column("X_NewsImageName")]
        [StringLength(100)]
        public string XNewsImageName { get; set; }
        [Column("D_NewsPostDate", TypeName = "datetime")]
        public DateTime? DNewsPostDate { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("X_NewsCode")]
        [StringLength(50)]
        public string XNewsCode { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
