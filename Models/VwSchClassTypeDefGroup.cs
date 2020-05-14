﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClassTypeDefGroup
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_StudentDefGroupID")]
        public int? NStudentDefGroupId { get; set; }
        [Column("N_StudentDefAccountID")]
        public int? NStudentDefAccountId { get; set; }
        [Column("N_FeeIncomeDefAccountID")]
        public int? NFeeIncomeDefAccountId { get; set; }
        [Column("N_FeeProposedIncomeDefAccountID")]
        public int? NFeeProposedIncomeDefAccountId { get; set; }
        [Column("X_StudentDefGroupCode")]
        [StringLength(50)]
        public string XStudentDefGroupCode { get; set; }
        [Column("X_StudentDefGroupName")]
        [StringLength(100)]
        public string XStudentDefGroupName { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
