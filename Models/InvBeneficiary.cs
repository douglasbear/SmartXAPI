using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Beneficiary")]
    public partial class InvBeneficiary
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BeneficiaryID")]
        public int NBeneficiaryId { get; set; }
        [Column("X_BeneficiaryCode")]
        [StringLength(50)]
        public string XBeneficiaryCode { get; set; }
        [Column("X_BeneficiaryName")]
        [StringLength(100)]
        public string XBeneficiaryName { get; set; }
        [Column("X_BeneficiaryAddress")]
        public string XBeneficiaryAddress { get; set; }
        [Column("X_BeneficiaryPhone")]
        [StringLength(20)]
        public string XBeneficiaryPhone { get; set; }
        [Column("X_BeneficiaryBank")]
        [StringLength(50)]
        public string XBeneficiaryBank { get; set; }
        [Column("X_BeneficiaryBranch")]
        [StringLength(100)]
        public string XBeneficiaryBranch { get; set; }
        [Column("X_BeneficiaryAccount")]
        [StringLength(50)]
        public string XBeneficiaryAccount { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_BeneficiarySwiftCode")]
        [StringLength(50)]
        public string XBeneficiarySwiftCode { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_FP_UserID")]
        public int? NFpUserId { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Column("X_BeneficiaryName_Ar")]
        [StringLength(100)]
        public string XBeneficiaryNameAr { get; set; }
    }
}
