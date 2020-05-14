using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Veh_FileMoving")]
    public partial class VehFileMoving
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("X_TransNo")]
        [StringLength(50)]
        public string XTransNo { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
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
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_SellerAmt", TypeName = "money")]
        public decimal? NSellerAmt { get; set; }
        [Column("N_BuyerAmt", TypeName = "money")]
        public decimal? NBuyerAmt { get; set; }
        [Column("N_CommnAmt", TypeName = "money")]
        public decimal? NCommnAmt { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("X_OwnerName")]
        [StringLength(100)]
        public string XOwnerName { get; set; }
        [Column("X_OwnerIqamaNo")]
        [StringLength(50)]
        public string XOwnerIqamaNo { get; set; }
        [Column("X_OwnerAddress")]
        [StringLength(150)]
        public string XOwnerAddress { get; set; }
        [Column("X_VehIDNo")]
        [StringLength(15)]
        public string XVehIdno { get; set; }
        [Column("X_VehRemarks")]
        [StringLength(100)]
        public string XVehRemarks { get; set; }
        [Column("N_VehNoOfRegns")]
        public int? NVehNoOfRegns { get; set; }
        [Column("N_OtherExpAmt", TypeName = "money")]
        public decimal? NOtherExpAmt { get; set; }
    }
}
