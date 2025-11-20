using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class SettingModel
    {
        public int Id { get; set; }
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }
    }
}