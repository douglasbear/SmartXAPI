using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Agent")]
    public partial class PayAgent
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_AgentID")]
        public int NAgentId { get; set; }
        [Required]
        [Column("X_AgentCode")]
        [StringLength(50)]
        public string XAgentCode { get; set; }
        [Required]
        [Column("X_AgentName")]
        [StringLength(100)]
        public string XAgentName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_ZipCode")]
        [StringLength(25)]
        public string XZipCode { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(20)]
        public string XPhoneNo { get; set; }
        [Required]
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
