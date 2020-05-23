using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class Acc_CompanyRepo : IAcc_CompanyRepo
    {
        private readonly SmartxContext _context;

        public Acc_CompanyRepo(SmartxContext context)
        {
            _context = context;
        }
        public IEnumerable<AccCompany> GetAllCompanys()
        {
            //return _context.AccCompany.ToList();
            return _context.AccCompany
            .Where(c=>!(bool)c.BInactive)
            .ToList();
        }

    }

    public interface IAcc_CompanyRepo
    {
        IEnumerable<AccCompany> GetAllCompanys();
    }
}