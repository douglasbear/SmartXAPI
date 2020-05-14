using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_ServiceEnd")]
    public partial class PayServiceEnd
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_ServiceEndID")]
        public int NServiceEndId { get; set; }
        [Column("X_ServiceEndCode")]
        [StringLength(20)]
        public string XServiceEndCode { get; set; }
        [Column("N_ServiceEndStatusID")]
        public int? NServiceEndStatusId { get; set; }
        [Column("X_ServiceEndStatusDesc")]
        [StringLength(200)]
        public string XServiceEndStatusDesc { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
        [Column("N_UpdatedUserID")]
        public int? NUpdatedUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
    }
}
