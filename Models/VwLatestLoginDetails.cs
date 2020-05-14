using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLatestLoginDetails
    {
        [Column("D_LoginTime")]
        [StringLength(10)]
        public string DLoginTime { get; set; }
        [Column("X_SystemName")]
        [StringLength(1000)]
        public string XSystemName { get; set; }
        [Column("X_UserID")]
        [StringLength(100)]
        public string XUserId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_LoginID")]
        public int NLoginId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("D_LoginDate", TypeName = "datetime")]
        public DateTime? DLoginDate { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
    }
}
