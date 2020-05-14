using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_ContactDetails")]
    public partial class PayContactDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ContactDetailsID")]
        public int NContactDetailsId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmergencyContctPerson")]
        [StringLength(100)]
        public string XEmergencyContctPerson { get; set; }
        [Column("X_EmergencyContctPersonH")]
        [StringLength(100)]
        public string XEmergencyContctPersonH { get; set; }
        [Column("X_EmergencyRelation")]
        [StringLength(50)]
        public string XEmergencyRelation { get; set; }
        [Column("X_EmergencyRelationH")]
        [StringLength(50)]
        public string XEmergencyRelationH { get; set; }
        [Column("X_EmergencyEmail")]
        [StringLength(100)]
        public string XEmergencyEmail { get; set; }
        [Column("X_EmergencyEmailH")]
        [StringLength(100)]
        public string XEmergencyEmailH { get; set; }
        [Column("X_EmergencyAddress")]
        public string XEmergencyAddress { get; set; }
        [Column("X_EmergencyAddressH")]
        public string XEmergencyAddressH { get; set; }
        [Column("X_EmergencyNum")]
        [StringLength(20)]
        public string XEmergencyNum { get; set; }
        [Column("X_EmergencyNumH")]
        [StringLength(20)]
        public string XEmergencyNumH { get; set; }
    }
}
