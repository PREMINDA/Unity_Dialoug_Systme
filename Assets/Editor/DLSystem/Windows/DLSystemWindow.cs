using Editor.Utils;
using UnityEditor;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


namespace Editor.DLSystem.Windows
{
    using UnityEditor.UIElements;
    public class DLSystemWindow : EditorWindow
    {
        private readonly string _defaultName = "DialoguesFileName";
        private Button _button;
        [MenuItem("Window/DLSystem/DLSystem Graph")]
        public static void ShowExample()
        {
            GetWindow<DLSystemWindow>("DLSystem Graph");
            
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolBar();
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();
            
            TextField textField = DLSystemUtils.CreateTextField(_defaultName,new string[]{"dls-fileName"},"File Name :");
            _button = DLSystemUtils.CreateButton("Save");
            toolbar.Add(textField);
            toolbar.Add(_button);
            StyleSheet styleSheet1 =
                (StyleSheet)EditorGUIUtility.Load(
                    "DLSystem/DLSystemToolBar.uss");
            toolbar.styleSheets.Add(styleSheet1);
            rootVisualElement.Add(toolbar);
        }   

        private void AddGraphView() 
        {
            DLSystemGraphView graphView = new DLSystemGraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
        
    }

}