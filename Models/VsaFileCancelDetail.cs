using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_FileCancelDetail")]
    public partial class VsaFileCancelDetail
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CancelId")]
        public int NCancelId { get; set; }
        [Key]
        [Column("N_CancelDetailId")]
        public int NCancelDetailId { get; set; }
        [Column("D_PayDate", TypeName = "smalldatetime")]
        public DateTime? DPayDate { get; set; }
        [Column("X_description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_RfdAmt", TypeName = "money")]
        public decimal? NRfdAmt { get; set; }
    }
}
