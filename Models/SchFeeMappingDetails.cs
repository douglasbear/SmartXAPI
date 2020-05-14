using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeMappingDetails")]
    public partial class SchFeeMappingDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_FeeMappingDetailsID")]
        public int NFeeMappingDetailsId { get; set; }
        [Column("N_FeeMappingID")]
        public int? NFeeMappingId { get; set; }
        [Column("N_FeeTypeID")]
        public int? NFeeTypeId { get; set; }
        [Column("N_FeeTypeIDTo")]
        public int? NFeeTypeIdto { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
