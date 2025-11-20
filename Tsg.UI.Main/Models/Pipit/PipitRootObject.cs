using System.Collections.Generic;

namespace Tsg.UI.Main.Models.Pipit
{
    public class PipitRootObject
    {
        public List<PipitContent> Content { get; set; }
        public bool First { get; set; }
        public object Sort { get; set; }
        public bool Last { get; set; }
        public int NumberOfElements { get; set; }
        public int Size { get; set; }
        public int Number { get; set; }
    }
}