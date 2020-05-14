using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_BusRoute")]
    public partial class SchBusRoute
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_RouteID")]
        public int NRouteId { get; set; }
        [Column("X_RouteNo")]
        [StringLength(50)]
        public string XRouteNo { get; set; }
        [Column("X_RouteName")]
        [StringLength(200)]
        public string XRouteName { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("D_Time")]
        [StringLength(100)]
        public string DTime { get; set; }
        [Column("N_BusID")]
        public int NBusId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
