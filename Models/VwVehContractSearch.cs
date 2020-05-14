using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVehContractSearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ContractID")]
        public int NContractId { get; set; }
        [Column("Contract No")]
        [StringLength(50)]
        public string ContractNo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Required]
        [Column("Vehicle Name")]
        [StringLength(100)]
        public string VehicleName { get; set; }
        [Required]
        [Column("Chase No")]
        [StringLength(50)]
        public string ChaseNo { get; set; }
        [Required]
        [Column("Seller Name")]
        [StringLength(100)]
        public string SellerName { get; set; }
        [Required]
        [Column("Seller Iqama No")]
        [StringLength(50)]
        public string SellerIqamaNo { get; set; }
        [Required]
        [Column("Seller Mobile No")]
        [StringLength(50)]
        public string SellerMobileNo { get; set; }
        [Required]
        [Column("Buyer Name")]
        [StringLength(100)]
        public string BuyerName { get; set; }
        [Required]
        [Column("Buyer Iqama No")]
        [StringLength(50)]
        public string BuyerIqamaNo { get; set; }
        [Required]
        [Column("Buyer Mobile No")]
        [StringLength(50)]
        public string BuyerMobileNo { get; set; }
        [Required]
        [Column("Plate No")]
        [StringLength(50)]
        public string PlateNo { get; set; }
    }
}
