using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class Division
    {
        [Column("DIVISION_ID")]
        public int DivisionId { get; set; }
        [Column("DIVISION_NAME")]
        [StringLength(50)]
        public string DivisionName { get; set; }
        [Column("DIVISION_CREATED_DATE", TypeName = "datetime")]
        public DateTime DivisionCreatedDate { get; set; }
        [Column("DIVISION_MODIFIED_DATE", TypeName = "datetime")]
        public DateTime DivisionModifiedDate { get; set; }
    }
}
