using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_Gen_Defaults")]
    public partial class FfwGenDefaults
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
        [Column("B_ShowDefault")]
        public bool? BShowDefault { get; set; }
    }
}
