using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_LatestLoginID")]
    public partial class GenLatestLoginId
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_LoginID")]
        public int? NLoginId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
    }
}
