using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class LanguageRepo : ILanguageRepo
    {
        private readonly IConfiguration _config;
        private readonly SmartxContext _context;
        private readonly IMapper _mapper;
        public LanguageRepo(SmartxContext context,IMapper mapper,IConfiguration config)
        {
            _context = context;
            _mapper=mapper;
            _config = config;
        }


        public IEnumerable<LanLanguage> GetLanguageList()
        {
            return _context.LanLanguage
            .ToList();
        }

     public Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetControllsListAsync()
        {
              var LanMultiLingual = _context.VwLanMultilingual
             .OrderBy(d=>d.NFormId)
             .ToList(); 
           
Dictionary<string,Dictionary<string,Dictionary<string,string>>> outputObj = new Dictionary<string,Dictionary<string,Dictionary<string,string>>>();
            foreach(int formID in LanMultiLingual.Select(x=>x.NFormId).Distinct()){
                Dictionary<string,Dictionary<string,string>> cntrlObj = new Dictionary<string,Dictionary<string,string>>();
                foreach(string CntrlID in LanMultiLingual.Where(y=>y.NFormId==formID).Select(y=>y.XControlNo).Distinct()){
                        Dictionary<string,string> langStringObj = new Dictionary<string,string>();
                        foreach(var ControllStringItem in LanMultiLingual.Where(y=>y.NFormId==formID && y.XControlNo==CntrlID)){
                                langStringObj.Add(ControllStringItem.EnglishId.ToString(),ControllStringItem.English);
                                langStringObj.Add(ControllStringItem.ArabicId.ToString(),ControllStringItem.Arabic);
                                break;
                            }
                        cntrlObj.Add(CntrlID.ToString(),langStringObj);
                    }
                outputObj.Add(formID.ToString(),cntrlObj);
            } 

            return outputObj;
           //}
        }

    }

    public interface ILanguageRepo
    {
        IEnumerable<LanLanguage> GetLanguageList();
        //IEnumerable<LanguageOutputDto> GetControllsList();
       // Dictionary<string,Dictionary<string,Dictionary<string,string>>> GetControllsListAsync();
       Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetControllsListAsync();
    }
}