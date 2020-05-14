using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_Defaults")]
    public partial class GenDefaults
    {
        [Key]
        [Column("N_TypeId")]
        public int NTypeId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
        [Column("X_TypeCode")]
        [StringLength(5)]
        public string XTypeCode { get; set; }
    }
}
