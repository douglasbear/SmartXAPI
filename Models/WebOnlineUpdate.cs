using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Web_OnlineUpdate")]
    public partial class WebOnlineUpdate
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("X_TableName")]
        [StringLength(100)]
        public string XTableName { get; set; }
        [Column("X_PKeyName")]
        [StringLength(100)]
        public string XPkeyName { get; set; }
        [Column("N_PKey")]
        public int? NPkey { get; set; }
        [Column("X_EntryType")]
        [StringLength(50)]
        public string XEntryType { get; set; }
        [Column("B_UpdateFlag")]
        public bool? BUpdateFlag { get; set; }
    }
}
