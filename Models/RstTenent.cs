using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rst_Tenent")]
    public partial class RstTenent
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_TenentID")]
        public int NTenentId { get; set; }
        [Column("X_TenentCode")]
        [StringLength(50)]
        public string XTenentCode { get; set; }
        [Column("X_TenentName")]
        [StringLength(400)]
        public string XTenentName { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("X_RoomNo")]
        [StringLength(50)]
        public string XRoomNo { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime DDateTo { get; set; }
        [Column("D_DepositAmount", TypeName = "money")]
        public decimal? DDepositAmount { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(50)]
        public string XPhoneNo { get; set; }
        [Column("N_FloorID")]
        public int? NFloorId { get; set; }
    }
}
