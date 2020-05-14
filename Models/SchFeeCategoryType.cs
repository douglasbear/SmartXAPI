using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeCategory_Type")]
    public partial class SchFeeCategoryType
    {
        [Key]
        [Column("N_FeeCategoryTypeID")]
        public int NFeeCategoryTypeId { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("X_Description")]
        [StringLength(25)]
        public string XDescription { get; set; }
    }
}
