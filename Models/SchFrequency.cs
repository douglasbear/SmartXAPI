using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_Frequency")]
    public partial class SchFrequency
    {
        public SchFrequency()
        {
            SchClassFeeSetup = new HashSet<SchClassFeeSetup>();
            SchFeeType = new HashSet<SchFeeType>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FrequencyID")]
        public int NFrequencyId { get; set; }
        [Required]
        [Column("X_FrequencyName")]
        [StringLength(15)]
        public string XFrequencyName { get; set; }
        [Column("N_Frequency")]
        public int NFrequency { get; set; }

        [InverseProperty("NFrequency")]
        public virtual ICollection<SchClassFeeSetup> SchClassFeeSetup { get; set; }
        [InverseProperty("NFrequencyNavigation")]
        public virtual ICollection<SchFeeType> SchFeeType { get; set; }
    }
}
