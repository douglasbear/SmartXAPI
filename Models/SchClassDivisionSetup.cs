using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_ClassDivisionSetup")]
    public partial class SchClassDivisionSetup
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ClassDivisionSetupID")]
        public int NClassDivisionSetupId { get; set; }
        [Column("N_ClassDivisionID")]
        public int NClassDivisionId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NClassId))]
        [InverseProperty(nameof(SchClass.SchClassDivisionSetup))]
        public virtual SchClass NClass { get; set; }
        [ForeignKey(nameof(NClassDivisionId))]
        [InverseProperty(nameof(SchClassDivision.SchClassDivisionSetup))]
        public virtual SchClassDivision NClassDivision { get; set; }
    }
}
