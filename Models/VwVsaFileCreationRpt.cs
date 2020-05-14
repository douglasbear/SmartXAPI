using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaFileCreationRpt
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
        public decimal? Amount { get; set; }
        [Column(TypeName = "money")]
        public decimal? Discount { get; set; }
        [Column("Contracted_Amount")]
        public int ContractedAmount { get; set; }
        [Required]
        [Column("Contracted_Date")]
        [StringLength(1)]
        public string ContractedDate { get; set; }
        [Column("Paid_Date", TypeName = "datetime")]
        public DateTime? PaidDate { get; set; }
        [Column("Received_Amount", TypeName = "money")]
        public decimal? ReceivedAmount { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
    }
}
