using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class Class
    {
        [Column("CLASS_ID")]
        public int ClassId { get; set; }
        [Column("CAMPUS_ID")]
        public int CampusId { get; set; }
        [Column("CLASS_NAME")]
        [StringLength(50)]
        public string ClassName { get; set; }
        [Column("Class_Description")]
        [StringLength(50)]
        public string ClassDescription { get; set; }
        [Column("CLASS_TYPE_ID")]
        public int? ClassTypeId { get; set; }
        [Required]
        [Column("Class_Capacity")]
        [StringLength(1)]
        public string ClassCapacity { get; set; }
        [Column("CLASS_CREATED_DATE", TypeName = "datetime")]
        public DateTime ClassCreatedDate { get; set; }
        [Column("CLASS_MODIFIED_DATE", TypeName = "datetime")]
        public DateTime ClassModifiedDate { get; set; }
    }
}
