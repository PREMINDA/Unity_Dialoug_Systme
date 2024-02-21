using System.Collections.Generic;
using UnityEngine;
using SerializableDictionary;
namespace DLSystem.Scripts.ScriptableObjects
{
    
    public class DLSystemContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<DLSystemDialogueGroupSO, List<DLSystemDialogueSO>> GroupDialogues { get; set; }
        [field: SerializeField] public List<DLSystemDialogueSO> UngroupedDialogues { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            GroupDialogues = new SerializableDictionary<DLSystemDialogueGroupSO, List<DLSystemDialogueSO>>();
            UngroupedDialogues = new List<DLSystemDialogueSO>();
        }
    }
}