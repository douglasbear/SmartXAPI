using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_MappingCashFlow")]
    public partial class AccMappingCashFlow
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_MappingID")]
        public int? NMappingId { get; set; }
        [Column("X_MappingCode")]
        [StringLength(50)]
        public string XMappingCode { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("B_IsAdd")]
        public bool? BIsAdd { get; set; }
        [Column("X_level")]
        [StringLength(10)]
        public string XLevel { get; set; }
    }
}
