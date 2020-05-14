using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_ProjectParameters")]
    public partial class PrjProjectParameters
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_PrjParametersID")]
        public int NPrjParametersId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("N_MappingID")]
        public int? NMappingId { get; set; }
        [Column("N_PayCodeID")]
        public int? NPayCodeId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_PriceTypeID")]
        public int? NPriceTypeId { get; set; }
        [Column("N_DefaultFeePayCodeID")]
        public int? NDefaultFeePayCodeId { get; set; }
        [Column("N_DefaultFeeAmount", TypeName = "money")]
        public decimal? NDefaultFeeAmount { get; set; }
    }
}
