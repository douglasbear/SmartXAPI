using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchBusRouteDetailDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_RouteID")]
        public int NRouteId { get; set; }
        [Column("N_RouteDetailID")]
        public int NRouteDetailId { get; set; }
        [Column("D_Time", TypeName = "datetime")]
        public DateTime DTime { get; set; }
        [Column("X_OrderNo")]
        [StringLength(200)]
        public string XOrderNo { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_StopID")]
        public int? NStopId { get; set; }
        [Column("X_StopCode")]
        [StringLength(20)]
        public string XStopCode { get; set; }
        [Column("X_StopName")]
        [StringLength(20)]
        public string XStopName { get; set; }
    }
}
