using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_Class")]
    public partial class SchClass
    {
        public SchClass()
        {
            SchAdmission = new HashSet<SchAdmission>();
            SchClassDivisionSetup = new HashSet<SchClassDivisionSetup>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_StudentAccountGroupID")]
        public int? NStudentAccountGroupId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_ClassCode")]
        [StringLength(20)]
        public string XClassCode { get; set; }

        [ForeignKey(nameof(NClassTypeId))]
        [InverseProperty(nameof(SchClassType.SchClass))]
        public virtual SchClassType NClassType { get; set; }
        [InverseProperty("NClass")]
        public virtual ICollection<SchAdmission> SchAdmission { get; set; }
        [InverseProperty("NClass")]
        public virtual ICollection<SchClassDivisionSetup> SchClassDivisionSetup { get; set; }
    }
}
