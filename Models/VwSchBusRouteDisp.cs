using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchBusRouteDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
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
        [Column("D_Time", TypeName = "datetime")]
        public DateTime DTime { get; set; }
        [Column("N_BusID")]
        public int NBusId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_VehicleCode")]
        [StringLength(50)]
        public string XVehicleCode { get; set; }
        [Column("X_VehicleName")]
        [StringLength(50)]
        public string XVehicleName { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
        [Column("X_TypeCode")]
        [StringLength(5)]
        public string XTypeCode { get; set; }
    }
}
