using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_ClassType")]
    public partial class SchClassType
    {
        public SchClassType()
        {
            SchClass = new HashSet<SchClass>();
            SchClassFeeSetup = new HashSet<SchClassFeeSetup>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_StudentDefGroupID")]
        public int? NStudentDefGroupId { get; set; }
        [Column("N_StudentDefAccountID")]
        public int? NStudentDefAccountId { get; set; }
        [Column("N_FeeIncomeDefAccountID")]
        public int? NFeeIncomeDefAccountId { get; set; }
        [Column("N_FeeProposedIncomeDefAccountID")]
        public int? NFeeProposedIncomeDefAccountId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_TransportFeeProposedIncomeDefAccountID")]
        public int? NTransportFeeProposedIncomeDefAccountId { get; set; }
        [Column("N_TransportFeeIncomeDefAccountID")]
        public int? NTransportFeeIncomeDefAccountId { get; set; }

        [InverseProperty("NClassType")]
        public virtual ICollection<SchClass> SchClass { get; set; }
        [InverseProperty("NClass")]
        public virtual ICollection<SchClassFeeSetup> SchClassFeeSetup { get; set; }
    }
}
