using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_BackupType")]
    public partial class GenBackupType
    {
        [Column("N_TypeID")]
        public long? NTypeId { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
    }
}
