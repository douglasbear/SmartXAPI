using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class ClassType
    {
        [Column("class_type_ID")]
        public int ClassTypeId { get; set; }
        [Column("CLASS_TYPE_NAME")]
        [StringLength(50)]
        public string ClassTypeName { get; set; }
        [Column("CLASS_TYPE_DESCRIPTION")]
        [StringLength(50)]
        public string ClassTypeDescription { get; set; }
        [Column("CLASS_TYPE_CREATED_DATE", TypeName = "datetime")]
        public DateTime ClassTypeCreatedDate { get; set; }
        [Column("CLASS_TYPE_MODIFIED_DATE", TypeName = "datetime")]
        public DateTime ClassTypeModifiedDate { get; set; }
    }
}
