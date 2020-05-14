using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class RelClassDivision
    {
        [Column("REL_CLASS_DIVISION")]
        public int RelClassDivision1 { get; set; }
        [Column("CLASS_ID")]
        public int ClassId { get; set; }
        [Column("DIVISION_ID")]
        public int DivisionId { get; set; }
        [Required]
        [Column("CLASS_DESCRIPTION")]
        [StringLength(1)]
        public string ClassDescription { get; set; }
        [Column("CLASS_TYPE_ID")]
        public int ClassTypeId { get; set; }
        [Column("CLASS_CAPACITY")]
        public int ClassCapacity { get; set; }
        [Column("CAMPUS_ID")]
        public int CampusId { get; set; }
        [Column("SCHOOL_ID")]
        public int SchoolId { get; set; }
        [Column("REL_CLASS_DIVISION_CREATED_DATE", TypeName = "datetime")]
        public DateTime RelClassDivisionCreatedDate { get; set; }
        [Column("REL_CLASS_DIVISION_MODIFIED_DATE", TypeName = "datetime")]
        public DateTime RelClassDivisionModifiedDate { get; set; }
    }
}
