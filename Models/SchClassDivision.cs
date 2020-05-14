using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_ClassDivision")]
    public partial class SchClassDivision
    {
        public SchClassDivision()
        {
            SchClassDivisionSetup = new HashSet<SchClassDivisionSetup>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ClassDivisionID")]
        public int NClassDivisionId { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [InverseProperty("NClassDivision")]
        public virtual ICollection<SchClassDivisionSetup> SchClassDivisionSetup { get; set; }
    }
}
