using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class MenuRepo : IMenuRepo
    {
        private readonly SmartxContext _context;

        public MenuRepo(SmartxContext context)
        {
            _context = context;
        }
        public IEnumerable<VwUserMenus> GetAllMenus()
        {
            //return _context.AccCompany.ToList();
            return _context.VwUserMenus
            .ToList();
        }

    }

    public interface IMenuRepo
    {
        IEnumerable<VwUserMenus> GetAllMenus();
    }
}