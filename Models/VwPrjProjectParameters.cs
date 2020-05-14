using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjProjectParameters
    {
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
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_GroupName")]
        [StringLength(50)]
        public string XGroupName { get; set; }
        [Column("X_MappingName")]
        [StringLength(50)]
        public string XMappingName { get; set; }
        [Column("X_MappingCode")]
        [StringLength(50)]
        public string XMappingCode { get; set; }
        [Column("X_GroupCode")]
        [StringLength(50)]
        public string XGroupCode { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DefaultFeePayCodeID")]
        public int NDefaultFeePayCodeId { get; set; }
        [Column("N_DefaultFeeAmount", TypeName = "money")]
        public decimal NDefaultFeeAmount { get; set; }
        [Column("X_FeeDescription")]
        [StringLength(100)]
        public string XFeeDescription { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_PriceTypeID")]
        public int? NPriceTypeId { get; set; }
    }
}
