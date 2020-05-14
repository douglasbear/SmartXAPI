using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVehFileMovingSearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("File No")]
        [StringLength(50)]
        public string FileNo { get; set; }
        [Column("File Date")]
        [StringLength(8000)]
        public string FileDate { get; set; }
        [Column("Vehicle Name")]
        [StringLength(100)]
        public string VehicleName { get; set; }
        [StringLength(50)]
        public string Model { get; set; }
        [Column("Chase No")]
        [StringLength(50)]
        public string ChaseNo { get; set; }
        [StringLength(50)]
        public string Color { get; set; }
        [Column("Serial No")]
        [StringLength(15)]
        public string SerialNo { get; set; }
        [Column("Seller Name")]
        [StringLength(100)]
        public string SellerName { get; set; }
        [Required]
        [Column("Seller Iqama No")]
        [StringLength(25)]
        public string SellerIqamaNo { get; set; }
        [Column("Buyer Name")]
        [StringLength(100)]
        public string BuyerName { get; set; }
        [Required]
        [Column("Buyer Iqama No")]
        [StringLength(25)]
        public string BuyerIqamaNo { get; set; }
        [Required]
        [Column("Plate No")]
        [StringLength(50)]
        public string PlateNo { get; set; }
    }
}
