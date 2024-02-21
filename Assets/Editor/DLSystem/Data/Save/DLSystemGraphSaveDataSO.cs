using System.Collections.Generic;
using UnityEngine;

namespace Editor.DLSystem.Data.Save
{
    using SerializableDictionary;
    public class DLSystemGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<DLSystemGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<DLSystemNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Groups = new List<DLSystemGroupSaveData>();
            Nodes = new List<DLSystemNodeSaveData>();
        }
    }
}