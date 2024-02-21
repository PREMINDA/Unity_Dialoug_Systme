using System;
using System.Collections.Generic;
using DLSystem.Enums;
using UnityEngine;

namespace Editor.DLSystem.Data.Save
{
    [Serializable]
    public class DLSystemNodeSaveData
    {
        [field:SerializeField]public string ID { get; set; }
        [field:SerializeField]public string Name { get; set; }
        [field:SerializeField]public string Text { get; set; }
        [field:SerializeField]public List<DLSystemChoiceSaveData> Choices { get; set; }
        [field:SerializeField]public string GroupId { get; set; }
        [field:SerializeField]public DLSystemType DialogType { get; set; }
        [field:SerializeField]public Vector2 Position { get; set; }
    }
}