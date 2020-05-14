using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_AdmissionClass")]
    public partial class SchAdmissionClass
    {
        [Key]
        [Column("N_AdmissionClassID")]
        public int NAdmissionClassId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("B_Active")]
        public bool BActive { get; set; }
        [Column("N_PromotionID")]
        public int? NPromotionId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
