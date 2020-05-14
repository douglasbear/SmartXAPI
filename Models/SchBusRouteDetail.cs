using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_BusRouteDetail")]
    public partial class SchBusRouteDetail
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_RouteID")]
        public int NRouteId { get; set; }
        [Key]
        [Column("N_RouteDetailID")]
        public int NRouteDetailId { get; set; }
        [Column("D_Time")]
        [StringLength(100)]
        public string DTime { get; set; }
        [Column("X_OrderNo")]
        [StringLength(200)]
        public string XOrderNo { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_StopID")]
        public int? NStopId { get; set; }
    }
}
