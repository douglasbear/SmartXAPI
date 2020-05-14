using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_RoomMaster")]
    public partial class PayRoomMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_RoomId")]
        public int NRoomId { get; set; }
        [Column("X_RoomCode")]
        [StringLength(100)]
        public string XRoomCode { get; set; }
        [Column("X_RoomName")]
        [StringLength(100)]
        public string XRoomName { get; set; }
        [Column("N_VillaID")]
        public int? NVillaId { get; set; }
        [Column("X_Location")]
        [StringLength(100)]
        public string XLocation { get; set; }
        [Column("N_RentAmount", TypeName = "money")]
        public decimal? NRentAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(300)]
        public string XRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_Electricity", TypeName = "money")]
        public decimal? NElectricity { get; set; }
        [Column("N_Water", TypeName = "money")]
        public decimal? NWater { get; set; }
        [Column("N_Internet", TypeName = "money")]
        public decimal? NInternet { get; set; }
        [Column("N_Capasity")]
        public int? NCapasity { get; set; }
    }
}
