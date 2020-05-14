using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_TruckMaster")]
    public partial class InvTruckMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TruckID")]
        public int NTruckId { get; set; }
        [Column("X_TruckCode")]
        [StringLength(100)]
        public string XTruckCode { get; set; }
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_OpeningMileage", TypeName = "numeric(18, 2)")]
        public decimal? NOpeningMileage { get; set; }
        [Column("N_CurrentMileage", TypeName = "numeric(18, 2)")]
        public decimal? NCurrentMileage { get; set; }
        [Column("X_Driver")]
        [StringLength(100)]
        public string XDriver { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(20)]
        public string XPhoneNo { get; set; }
        [Column("B_OwnProduct")]
        public bool? BOwnProduct { get; set; }
        [Column("N_AssetID")]
        public int? NAssetId { get; set; }
        [Column("N_DriverID")]
        public int? NDriverId { get; set; }
    }
}
