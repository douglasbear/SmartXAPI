using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_DriverRegistration")]
    public partial class SchDriverRegistration
    {
        public SchDriverRegistration()
        {
            SchVehicleExpence = new HashSet<SchVehicleExpence>();
            SchVehicleRegistration = new HashSet<SchVehicleRegistration>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_DriverID")]
        public int NDriverId { get; set; }
        [Column("X_DriverName")]
        [StringLength(50)]
        public string XDriverName { get; set; }
        [Column("X_IqamaNumber")]
        [StringLength(50)]
        public string XIqamaNumber { get; set; }
        [Column("X_DrivingLicenseID")]
        [StringLength(50)]
        public string XDrivingLicenseId { get; set; }
        [Column("N_NationalityID")]
        public int? NNationalityId { get; set; }

        [ForeignKey(nameof(NNationalityId))]
        [InverseProperty(nameof(PayNationality.SchDriverRegistration))]
        public virtual PayNationality NNationality { get; set; }
        [InverseProperty("NDriver")]
        public virtual ICollection<SchVehicleExpence> SchVehicleExpence { get; set; }
        [InverseProperty("NDriver")]
        public virtual ICollection<SchVehicleRegistration> SchVehicleRegistration { get; set; }
    }
}
