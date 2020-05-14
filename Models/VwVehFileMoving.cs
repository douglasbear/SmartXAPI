using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVehFileMoving
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
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
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_BuyerAmt", TypeName = "money")]
        public decimal? NBuyerAmt { get; set; }
        [Column("N_CommnAmt", TypeName = "money")]
        public decimal? NCommnAmt { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("X_BuyNo")]
        [StringLength(50)]
        public string XBuyNo { get; set; }
        [Column("X_BuyName")]
        [StringLength(100)]
        public string XBuyName { get; set; }
        [Column("X_BuyAddress")]
        [StringLength(250)]
        public string XBuyAddress { get; set; }
        [Column("X_BuyNationality")]
        [StringLength(50)]
        public string XBuyNationality { get; set; }
        [Column("X_BuyMobileNo")]
        [StringLength(20)]
        public string XBuyMobileNo { get; set; }
        [Column("X_BuyIqamaNo")]
        [StringLength(25)]
        public string XBuyIqamaNo { get; set; }
        [Column("D_BuyIqamaExpiry")]
        [StringLength(25)]
        public string DBuyIqamaExpiry { get; set; }
        [Column("X_BuyIqamaIssue")]
        [StringLength(25)]
        public string XBuyIqamaIssue { get; set; }
        [Column("N_SellerAmt", TypeName = "money")]
        public decimal? NSellerAmt { get; set; }
        [Column("X_VehBookSource")]
        [StringLength(100)]
        public string XVehBookSource { get; set; }
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
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_SelNo")]
        [StringLength(50)]
        public string XSelNo { get; set; }
        [Column("X_SelName")]
        [StringLength(100)]
        public string XSelName { get; set; }
        [Column("X_SelAddress")]
        [StringLength(250)]
        public string XSelAddress { get; set; }
        [Column("X_SelNationality")]
        [StringLength(50)]
        public string XSelNationality { get; set; }
        [Column("X_SelMobileNo")]
        [StringLength(20)]
        public string XSelMobileNo { get; set; }
        [Column("X_SelIqamaNo")]
        [StringLength(25)]
        public string XSelIqamaNo { get; set; }
        [Column("D_SelIqamaExpiry")]
        [StringLength(25)]
        public string DSelIqamaExpiry { get; set; }
        [Column("X_SelIqamaIssue")]
        [StringLength(25)]
        public string XSelIqamaIssue { get; set; }
        [Column("D_SelFnYearID")]
        public int? DSelFnYearId { get; set; }
        [Column("File Date")]
        [StringLength(8000)]
        public string FileDate { get; set; }
        [Column("File No")]
        [StringLength(50)]
        public string FileNo { get; set; }
    }
}
