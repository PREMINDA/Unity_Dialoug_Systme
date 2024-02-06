using System;
using UnityEditor.Experimental.GraphView;

namespace Editor.DLSystem.Entity
{
    public class IdGroup : Group
    {
        public Guid GroupId { get; set; }
        public IdGroup():base()
        {
        
        }
    }
}