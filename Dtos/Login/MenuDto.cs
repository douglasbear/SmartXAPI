using System.Collections.Generic;

namespace SmartxAPI.Dtos.Login
{
    public class MenuDto
    {
        public int NMenuId { get; set; }
        public string XMenuName { get; set; }
        public string XCaption { get; set; }
        public int? NParentMenuId { get; set; }
        public double? NOrder { get; set; }
        public bool? NHasChild { get; set; }
        public bool? BVisible { get; set; }
        public bool? BEdit { get; set; }
        public bool? BDelete { get; set; }
        public bool? BSave { get; set; }
        public bool? BView { get; set; }
        public string XShortcutKey { get; set; }
        public string XCaptionAr { get; set; }
        public string XFormNameWithTag { get; set; }
        public bool? NIsStartup { get; set; }
        public bool? BShow { get; set; }
        public string XRouteName { get; set; }
        public bool? BShowOnline { get; set; }
        public List<ChildMenuDto> ChildMenu {get; set;}
    }
}
