using System;
using UnityEngine;

namespace Editor.DLSystem.Data.Save
{
    [Serializable]
    public class DLSystemChoiceSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string NextNodeID { get; set; }
    }
}