using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_MailDetails")]
    public partial class GenMailDetails
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_MailID")]
        public int NMailId { get; set; }
        [Required]
        [Column("X_Tomail")]
        [StringLength(100)]
        public string XTomail { get; set; }
        [Column("X_Subject")]
        [StringLength(1000)]
        public string XSubject { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_CCMail")]
        [StringLength(1000)]
        public string XCcmail { get; set; }
        [Column("X_BCCMail")]
        [StringLength(1000)]
        public string XBccmail { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
    }
}
