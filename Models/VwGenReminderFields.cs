using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGenReminderFields
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_FieldNo")]
        [StringLength(100)]
        public string XFieldNo { get; set; }
        [Column("X_Text")]
        [StringLength(8000)]
        public string XText { get; set; }
        [Column("N_LanguageId")]
        public int NLanguageId { get; set; }
        [Required]
        [Column("X_Subject")]
        [StringLength(200)]
        public string XSubject { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
    }
}
