using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_Airport")]
    public partial class FfwAirport
    {
        public FfwAirport()
        {
            FfwInvoiceMasterNArrival = new HashSet<FfwInvoiceMaster>();
            FfwInvoiceMasterNDeparture = new HashSet<FfwInvoiceMaster>();
            FfwQuotationMasterNArrival = new HashSet<FfwQuotationMaster>();
            FfwQuotationMasterNDeparture = new HashSet<FfwQuotationMaster>();
        }

        [Key]
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
        [Column("X_TypeOfContainer")]
        [StringLength(100)]
        public string XTypeOfContainer { get; set; }
        [Column("N_NoOfContainer")]
        public int? NNoOfContainer { get; set; }

        [InverseProperty(nameof(FfwInvoiceMaster.NArrival))]
        public virtual ICollection<FfwInvoiceMaster> FfwInvoiceMasterNArrival { get; set; }
        [InverseProperty(nameof(FfwInvoiceMaster.NDeparture))]
        public virtual ICollection<FfwInvoiceMaster> FfwInvoiceMasterNDeparture { get; set; }
        [InverseProperty(nameof(FfwQuotationMaster.NArrival))]
        public virtual ICollection<FfwQuotationMaster> FfwQuotationMasterNArrival { get; set; }
        [InverseProperty(nameof(FfwQuotationMaster.NDeparture))]
        public virtual ICollection<FfwQuotationMaster> FfwQuotationMasterNDeparture { get; set; }
    }
}
