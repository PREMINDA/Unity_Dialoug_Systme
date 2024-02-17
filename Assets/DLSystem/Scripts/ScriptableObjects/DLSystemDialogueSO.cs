using System.Collections.Generic;
using DLSystem.Enums;
using DLSystem.Scripts.Data;
using UnityEditor;
using UnityEngine;

namespace DLSystem.Scripts.ScriptableObjects
{
    public class DLSystemDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<DLSystemDialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DLSystemType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }

        public void Initialize(string dialogueName, string text, List<DLSystemDialogueChoiceData> choices, DLSystemType dialogueType, bool isStartingDialogue)
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
        }
    }
} 