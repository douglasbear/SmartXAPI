using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccGroupWiseTotalByMonth
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_MappingCode")]
        [StringLength(50)]
        public string XMappingCode { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column("X_level")]
        [StringLength(10)]
        public string XLevel { get; set; }
        [Column("N_Month")]
        [StringLength(30)]
        public string NMonth { get; set; }
    }
}
