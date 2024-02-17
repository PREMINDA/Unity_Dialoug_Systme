using System;
using DLSystem.Scripts.ScriptableObjects;
using UnityEngine;

namespace DLSystem.Scripts.Data
{
    [Serializable]
    public class DLSystemDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DLSystemDialogueSO NextDialogue { get; set; }
    }
}