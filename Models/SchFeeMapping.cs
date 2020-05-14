using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeMapping")]
    public partial class SchFeeMapping
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_FeeMappingID")]
        public int NFeeMappingId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_ClassTypeIDTo")]
        public int? NClassTypeIdto { get; set; }
        [Column("N_ClassIDTo")]
        public int? NClassIdto { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
