using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Editor.DLSystem.Entity
{
    public class DLSystemMultiPortContainerBox : Box
    {
        public Port ConfigPort { get; set; }

        public DLSystemMultiPortContainerBox(Port port):base()
        {
            ConfigPort = port;
            Add(port);
        }
        
    }
}