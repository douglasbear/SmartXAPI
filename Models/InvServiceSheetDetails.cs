using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ServiceSheetDetails")]
    public partial class InvServiceSheetDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ServiceSheetID")]
        public int NServiceSheetId { get; set; }
        [Key]
        [Column("N_ServiceSheetDetailsID")]
        public int NServiceSheetDetailsId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_Hour")]
        public double? NHour { get; set; }
        [Column("N_ItemID")]
        public double? NItemId { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
