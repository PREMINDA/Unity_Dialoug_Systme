using UnityEngine;

namespace DLSystem.Scripts.ScriptableObjects
{
    public class DLSystemDialogueGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }

        public void Initialize(string groupName)
        {
            GroupName = groupName;
        }
    }
}