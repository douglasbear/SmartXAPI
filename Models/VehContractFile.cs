using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Veh_ContractFile")]
    public partial class VehContractFile
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ContractID")]
        public int NContractId { get; set; }
        [Column("X_ContractNo")]
        [StringLength(50)]
        public string XContractNo { get; set; }
        [Column("D_ContractDate", TypeName = "datetime")]
        public DateTime? DContractDate { get; set; }
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
        [Column("X_BuyName")]
        [StringLength(100)]
        public string XBuyName { get; set; }
        [Column("X_BuyNationality")]
        [StringLength(50)]
        public string XBuyNationality { get; set; }
        [Column("X_BuyIqamaNo")]
        [StringLength(50)]
        public string XBuyIqamaNo { get; set; }
        [Column("D_BuyIqamaExpiry")]
        [StringLength(25)]
        public string DBuyIqamaExpiry { get; set; }
        [Column("X_BuyIqamaIssue")]
        [StringLength(50)]
        public string XBuyIqamaIssue { get; set; }
        [Column("X_BuyMobileNo")]
        [StringLength(50)]
        public string XBuyMobileNo { get; set; }
        [Column("X_BuyAddress")]
        [StringLength(150)]
        public string XBuyAddress { get; set; }
        [Column("X_SelName")]
        [StringLength(100)]
        public string XSelName { get; set; }
        [Column("X_SelNationality")]
        [StringLength(50)]
        public string XSelNationality { get; set; }
        [Column("X_SelIqamaNo")]
        [StringLength(50)]
        public string XSelIqamaNo { get; set; }
        [Column("D_SelIqamaExpiry")]
        [StringLength(25)]
        public string DSelIqamaExpiry { get; set; }
        [Column("X_SelIqamaIssue")]
        [StringLength(50)]
        public string XSelIqamaIssue { get; set; }
        [Column("X_SelMobileNo")]
        [StringLength(50)]
        public string XSelMobileNo { get; set; }
        [Column("X_SelNotes")]
        [StringLength(150)]
        public string XSelNotes { get; set; }
        [Column("N_Total", TypeName = "money")]
        public decimal? NTotal { get; set; }
        [Column("X_OldVehBookName")]
        [StringLength(100)]
        public string XOldVehBookName { get; set; }
        [Column("X_VehIDNo")]
        [StringLength(15)]
        public string XVehIdno { get; set; }
        [Column("X_VehRemarks")]
        [StringLength(100)]
        public string XVehRemarks { get; set; }
        [Column("N_VehNoOfRegns")]
        public int? NVehNoOfRegns { get; set; }
        [Column("N_TotalInWords")]
        [StringLength(5000)]
        public string NTotalInWords { get; set; }
    }
}
