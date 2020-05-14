using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_FlightMaster")]
    public partial class FfwFlightMaster
    {
        [Key]
        [Column("N_FlightID")]
        public int NFlightId { get; set; }
        [Required]
        [Column("X_FlightCode")]
        [StringLength(50)]
        public string XFlightCode { get; set; }
        [Column("X_FlightName")]
        [StringLength(100)]
        public string XFlightName { get; set; }
        [Column("X_FlightNumber")]
        [StringLength(100)]
        public string XFlightNumber { get; set; }
        [Column("X_Country")]
        [StringLength(45)]
        public string XCountry { get; set; }
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
