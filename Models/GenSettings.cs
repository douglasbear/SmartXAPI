using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_Settings")]
    public partial class GenSettings
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Required]
        [Column("X_Group")]
        [StringLength(50)]
        public string XGroup { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_Value")]
        public int? NValue { get; set; }
        [Column("X_Value")]
        public string XValue { get; set; }
        [Column("N_UserCategoryID")]
        public int? NUserCategoryId { get; set; }
    }
}
