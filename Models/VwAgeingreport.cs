using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAgeingreport
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("File_No")]
        [StringLength(50)]
        public string FileNo { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        [Column(TypeName = "money")]
        public decimal Discount { get; set; }
        [Column("Contracted_Amount", TypeName = "money")]
        public decimal? ContractedAmount { get; set; }
        [Column("Contracted_Date", TypeName = "smalldatetime")]
        public DateTime ContractedDate { get; set; }
        [Column("N_ItemCode")]
        public int? NItemCode { get; set; }
        [Column("N_PaymentID")]
        public int NPaymentId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("N_ConsultantID")]
        public int? NConsultantId { get; set; }
        [Column("N_ParalegalID")]
        public int? NParalegalId { get; set; }
        [Column("X_ConsultantName")]
        [StringLength(60)]
        public string XConsultantName { get; set; }
        [Column("X_ParalegalName")]
        [StringLength(60)]
        public string XParalegalName { get; set; }
        [Column("N_contryid")]
        public int? NContryid { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
    }
}
