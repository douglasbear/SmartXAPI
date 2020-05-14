using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVehAuctionSearch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AuctionID")]
        public int NAuctionId { get; set; }
        [Column("Auction No")]
        [StringLength(50)]
        public string AuctionNo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("Vehicle Name")]
        [StringLength(100)]
        public string VehicleName { get; set; }
        [Column("Chase No")]
        [StringLength(50)]
        public string ChaseNo { get; set; }
        [Column("Owner Name")]
        [StringLength(100)]
        public string OwnerName { get; set; }
        [StringLength(50)]
        public string Model { get; set; }
        [Column("Plate No")]
        [StringLength(50)]
        public string PlateNo { get; set; }
        [StringLength(50)]
        public string Color { get; set; }
        [Column("Mobile No")]
        [StringLength(50)]
        public string MobileNo { get; set; }
        [Column("Serial No")]
        [StringLength(15)]
        public string SerialNo { get; set; }
        [Column("X_AuctionNo")]
        [StringLength(50)]
        public string XAuctionNo { get; set; }
    }
}
