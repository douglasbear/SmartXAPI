
using System.Collections.Generic;

namespace SmartxAPI.Dtos.Language
{
    public partial class LanguageOutputDto
    {
        //public int ScreenID { get; set; }
        //public IEnumerable<Controlls> Controlls { get; set; }
        public Dictionary<string, Controlls> Controls { get; set; }
    }
}
