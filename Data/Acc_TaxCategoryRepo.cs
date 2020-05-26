using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class Acc_TaxCategoryRepo : IAccTaxCategoryRepo
    {
        private readonly SmartxContext _context;

        public Acc_TaxCategoryRepo(SmartxContext context)
        {
            _context = context;
        }
        public IEnumerable<AccTaxCategory> GetAllTaxTypes(int? nCompanyID)
        {
            //return _context.AccCompany.ToList();
            return _context.AccTaxCategory
            .Where(V => V.NCompanyId==nCompanyID)
            .ToList();
        }


    }

    public interface IAccTaxCategoryRepo
    {
        IEnumerable<AccTaxCategory> GetAllTaxTypes(int? nCompanyID);
    }
}