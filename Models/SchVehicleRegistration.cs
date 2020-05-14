using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_VehicleRegistration")]
    public partial class SchVehicleRegistration
    {
        public SchVehicleRegistration()
        {
            SchVehicleExpence = new HashSet<SchVehicleExpence>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_VehicleID")]
        public int NVehicleId { get; set; }
        [Column("X_VehicleCode")]
        [StringLength(50)]
        public string XVehicleCode { get; set; }
        [Column("X_VehicleName")]
        [StringLength(50)]
        public string XVehicleName { get; set; }
        [Column("X_NumberPlate")]
        [StringLength(50)]
        public string XNumberPlate { get; set; }
        [Column("N_Capacity")]
        public int? NCapacity { get; set; }
        [Column("X_Root")]
        [StringLength(50)]
        public string XRoot { get; set; }
        [Column("N_DriverID")]
        public int? NDriverId { get; set; }

        [ForeignKey(nameof(NDriverId))]
        [InverseProperty(nameof(SchDriverRegistration.SchVehicleRegistration))]
        public virtual SchDriverRegistration NDriver { get; set; }
        [InverseProperty("NVehicle")]
        public virtual ICollection<SchVehicleExpence> SchVehicleExpence { get; set; }
    }
}
