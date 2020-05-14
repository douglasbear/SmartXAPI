using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAirPort
    {
        [Column("N_AirportID")]
        public int NAirportId { get; set; }
        [Required]
        [Column("X_AirportCode")]
        [StringLength(50)]
        public string XAirportCode { get; set; }
        [Column("X_AirportName")]
        [StringLength(100)]
        public string XAirportName { get; set; }
        [Column("X_Country")]
        [StringLength(100)]
        public string XCountry { get; set; }
        [Column("B_DefAir")]
        public bool? BDefAir { get; set; }
        [Column("N_ModeOfOperation")]
        public int? NModeOfOperation { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_TypeId")]
        public int NTypeId { get; set; }
        [Required]
        [Column("X_TypeOfContainer")]
        [StringLength(100)]
        public string XTypeOfContainer { get; set; }
        [Column("N_NoOfContainer")]
        public int NNoOfContainer { get; set; }
    }
}
