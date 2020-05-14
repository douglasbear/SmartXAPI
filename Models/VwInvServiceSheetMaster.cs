﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvServiceSheetMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("N_ServiceSheetID")]
        public int NServiceSheetId { get; set; }
        [Required]
        [Column("X_ServiceSheetCode")]
        [StringLength(100)]
        public string XServiceSheetCode { get; set; }
        [Column("D_Invoicedate", TypeName = "datetime")]
        public DateTime DInvoicedate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_Type")]
        [StringLength(100)]
        public string XType { get; set; }
        [Column("D_Transdate")]
        [StringLength(100)]
        public string DTransdate { get; set; }
        [Column("N_SOFnYearID")]
        public int? NSofnYearId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_Month")]
        public int? NMonth { get; set; }
    }
}
