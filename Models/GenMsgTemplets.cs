using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_Msg_Templets")]
    public partial class GenMsgTemplets
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("X_Code")]
        [StringLength(50)]
        public string XCode { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("X_ShortName")]
        [StringLength(100)]
        public string XShortName { get; set; }
        [Column("X_Templets")]
        [StringLength(500)]
        public string XTemplets { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
    }
}
