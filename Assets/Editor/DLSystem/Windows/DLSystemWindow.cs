using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DLSystem.Windows
{
    public class DLSystemWindow : EditorWindow
    {
        [MenuItem("Window/DLSystem/DLSystem Graph")]
        public static void ShowExample()
        {
            GetWindow<DLSystemWindow>("DLSystem Graph");
            
        }

        private void OnEnable()
        {
            AddGraphView();
        }

        private void AddGraphView()
        {
            DLSystemGraphView graphView = new DLSystemGraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
        
    }

}