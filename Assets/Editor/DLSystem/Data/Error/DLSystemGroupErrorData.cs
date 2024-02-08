using System.Collections.Generic;
using Editor.DLSystem.Entity;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.DLSystem.Data.Error
{
    public class DLSystemGroupErrorData
    {
        public DLSystemErrorData ErrorData;
        public List<DLSystemGroup> Groups;


        public DLSystemGroupErrorData()
        {
            ErrorData = new DLSystemErrorData();
            Groups = new List<DLSystemGroup>();
        }
    }
}