using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TSG.Models.ServiceModels.Shop;

namespace Tsg.UI.Main.Methods
{
    public static class CommonMethods
    {
        public static List<SelectListItem> GetAllCategories(List<ShopCategories> list, Guid? id = null)
        {
            if (list == null)
                return new List<SelectListItem>();
            var res = new List<SelectListItem>
            {
                new SelectListItem {Value = null, Text = "Select parent category", Selected = true}
            };
            res.AddRange(list.Where(w => w.ShopCategories_ID != id).Select(s => new SelectListItem()
            {
                Value = s.ShopCategories_ID.ToString(),
                Text = s.ShopCategories_Name,
            }).ToList());
            if (id != null)
            {
                res.ForEach(f => { f.Selected = f.Value == id.ToString(); });
            }
            return res.OrderBy(ob => ob.Value).ToList();
        }

        public static List<SelectListItem> GetAllShops(List<ShopInfo> list, Guid? id = null)
        {
            if (list == null)
                return new List<SelectListItem>();
            var res = new List<SelectListItem>
            {
                new SelectListItem {Value = null, Text = "Select shops", Selected = true}
            };
            res.AddRange(list.Where(w => w.ShopInfo_ID != id).Select(s => new SelectListItem()
            {
                Value = s.ShopInfo_ID.ToString(),
                Text = s.ShopInfo_Name
            }).ToList());
            if (id != null)
            {
                res.ForEach(f => { f.Selected = f.Value == id.ToString(); });
            }
            return res.OrderBy(ob => ob.Value).ToList();
        }
        
    }
}