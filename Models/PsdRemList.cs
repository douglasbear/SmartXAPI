using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__PSD_RemList")]
    public partial class PsdRemList
    {
        [Column("Student_Name")]
        [StringLength(300)]
        public string StudentName { get; set; }
        [Column("sysyr")]
        [StringLength(200)]
        public string Sysyr { get; set; }
        [StringLength(100)]
        public string Schooladd { get; set; }
        [Column("prvgrade")]
        [StringLength(100)]
        public string Prvgrade { get; set; }
    }
}
