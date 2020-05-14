using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchVehicleRegistrationDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VehicleID")]
        public int NVehicleId { get; set; }
        [Column("Vehicle Code")]
        [StringLength(50)]
        public string VehicleCode { get; set; }
        [Column("Vehicle Name")]
        [StringLength(50)]
        public string VehicleName { get; set; }
    }
}
