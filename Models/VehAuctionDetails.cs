using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Veh_AuctionDetails")]
    public partial class VehAuctionDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_AuctionID")]
        public int NAuctionId { get; set; }
        [Column("X_AuctionNo")]
        [StringLength(50)]
        public string XAuctionNo { get; set; }
        [Column("D_AuctionDate", TypeName = "datetime")]
        public DateTime? DAuctionDate { get; set; }
        [Column("N_VehModel")]
        [StringLength(50)]
        public string NVehModel { get; set; }
        [Column("X_VehName")]
        [StringLength(100)]
        public string XVehName { get; set; }
        [Column("X_VehRegNo")]
        [StringLength(50)]
        public string XVehRegNo { get; set; }
        [Column("X_VehChaseNo")]
        [StringLength(50)]
        public string XVehChaseNo { get; set; }
        [Column("X_VehSize")]
        [StringLength(50)]
        public string XVehSize { get; set; }
        [Column("X_VehColor")]
        [StringLength(50)]
        public string XVehColor { get; set; }
        [Column("X_VehBookNo")]
        [StringLength(50)]
        public string XVehBookNo { get; set; }
        [Column("X_VehBookOwner")]
        [StringLength(100)]
        public string XVehBookOwner { get; set; }
        [Column("X_VehBookSource")]
        [StringLength(100)]
        public string XVehBookSource { get; set; }
        [Column("X_OwnerName")]
        [StringLength(100)]
        public string XOwnerName { get; set; }
        [Column("X_OwnerMobileNo")]
        [StringLength(50)]
        public string XOwnerMobileNo { get; set; }
        [Column("X_Status")]
        [StringLength(50)]
        public string XStatus { get; set; }
        [Column("X_VehIDNo")]
        [StringLength(15)]
        public string XVehIdno { get; set; }
        [Column("X_VehRemarks")]
        [StringLength(100)]
        public string XVehRemarks { get; set; }
        [Column("N_VehNoOfRegns")]
        public int? NVehNoOfRegns { get; set; }
    }
}
