using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentStatus")]
    public partial class SchStudentStatus
    {
        [Column("Status_Id")]
        public int? StatusId { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
    }
}
