using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_ReminderSettings")]
    public partial class GenReminderSettings
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_FieldNo")]
        [StringLength(100)]
        public string XFieldNo { get; set; }
        [Column("X_Subject")]
        [StringLength(200)]
        public string XSubject { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
    }
}
