using System.Collections.Generic;
using Editor.DLSystem.Elements;

namespace Editor.DLSystem.Data.Error
{
    public class DLSystemNodeErrorData
    {
        public DLSystemErrorData ErrorData { get; set; }
        public List<DLSystemNode> Nodes { get; set; }

        public DLSystemNodeErrorData()
        {
            ErrorData = new DLSystemErrorData();
            Nodes = new List<DLSystemNode>();
        }
    }
}