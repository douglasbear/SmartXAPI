using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Veh_Drivers")]
    public partial class VehDrivers
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_DriversID")]
        public int NDriversId { get; set; }
        [Column("X_DriversCode")]
        [StringLength(20)]
        public string XDriversCode { get; set; }
        [Column("X_DriversName")]
        [StringLength(200)]
        public string XDriversName { get; set; }
        [Column("X_Address")]
        [StringLength(500)]
        public string XAddress { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(100)]
        public string XPhoneNo { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_EmpRefID")]
        public int? NEmpRefId { get; set; }
    }
}
