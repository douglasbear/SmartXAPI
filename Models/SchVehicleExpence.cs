using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_VehicleExpence")]
    public partial class SchVehicleExpence
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_VehicleExpenseID")]
        public int NVehicleExpenseId { get; set; }
        [Column("N_VehicleID")]
        public int? NVehicleId { get; set; }
        [Column("N_DriverID")]
        public int? NDriverId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Note")]
        [StringLength(50)]
        public string XNote { get; set; }

        [ForeignKey(nameof(NDriverId))]
        [InverseProperty(nameof(SchDriverRegistration.SchVehicleExpence))]
        public virtual SchDriverRegistration NDriver { get; set; }
        [ForeignKey(nameof(NVehicleId))]
        [InverseProperty(nameof(SchVehicleRegistration.SchVehicleExpence))]
        public virtual SchVehicleRegistration NVehicle { get; set; }
    }
}
