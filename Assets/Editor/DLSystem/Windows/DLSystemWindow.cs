using Editor.DLSystem.Utils;
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
        private TextField _textField; 
        private DLSystemGraphView _graphView;
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
            
            _textField = DLSystemUtils.CreateTextField(_defaultName,new string[]{"dls-fileName"},"File Name :");
            _button = DLSystemUtils.CreateButton("Save",() =>Save() );
            toolbar.Add(_textField);
            toolbar.Add(_button);
            StyleSheet styleSheet1 =
                (StyleSheet)EditorGUIUtility.Load(
                    "DLSystem/DLSystemToolBar.uss");
            toolbar.styleSheets.Add(styleSheet1);
            rootVisualElement.Add(toolbar);
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_textField.value))
            {
                EditorUtility.DisplayDialog(
                    "Invalid File Name",
                    "Please Ensure the File Name",
                    "OK"
                    );
                return;
            }

            DLSystemIOUtils.Initialize(_textField.value,_graphView);
            DLSystemIOUtils.Save();
        }

        private void AddGraphView() 
        {
            _graphView = new DLSystemGraphView();
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }
        
    }

}