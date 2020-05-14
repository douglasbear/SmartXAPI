using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBusDetailsDisp
    {
        [Column("X_RouteNo")]
        [StringLength(50)]
        public string XRouteNo { get; set; }
        [Column("X_RouteName")]
        [StringLength(200)]
        public string XRouteName { get; set; }
        [Column("N_RouteID")]
        public int NRouteId { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("N_VehicleID")]
        public int NVehicleId { get; set; }
        [Column("X_VehicleCode")]
        [StringLength(50)]
        public string XVehicleCode { get; set; }
        [Column("X_VehicleName")]
        [StringLength(50)]
        public string XVehicleName { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_DriverName")]
        [StringLength(50)]
        public string XDriverName { get; set; }
    }
}
