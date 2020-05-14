using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ServiceContract")]
    public partial class InvServiceContract
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_ContractID")]
        public int NContractId { get; set; }
        [Column("X_ContractNo")]
        [StringLength(20)]
        public string XContractNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Required]
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("N_ItemId")]
        public int NItemId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_ContractDetailsID")]
        public int? NContractDetailsId { get; set; }
        [Column("N_SalesDetailsID")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_WarrantyID")]
        public int? NWarrantyId { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("N_DurationID")]
        public double? NDurationId { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
    }
}
