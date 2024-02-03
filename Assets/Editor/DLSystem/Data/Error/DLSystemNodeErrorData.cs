using System.Collections.Generic;
using Editor.DLSystem.Elements;

namespace Editor.DLSystem.Data.Error
{
    public class DLSystemNodeErrorData
    {
        public DLSystemErrorData ErrorData { get; set; }
        public List<DLSystemNode> DLSystemNodes { get; set; }

        public DLSystemNodeErrorData()
        {
            ErrorData = new DLSystemErrorData();
            DLSystemNodes = new List<DLSystemNode>();
        }
    }
}