using Editor.DLSystem.Utils;
using Editor.Utils;
using PlasticGui;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


namespace Editor.DLSystem.Windows
{
    using UnityEditor.UIElements;
    public class DLSystemWindow : EditorWindow
    {
        private readonly string _defaultName = "DialoguesFileName";
        private Button _button;
        private static TextField _textField; 
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

            Button clearButton = DLSystemUtils.CreateButton("Clear", () => Clear());
            Button resetButton = DLSystemUtils.CreateButton("Reset", () => ResetGraph());
            Button loadButton = DLSystemUtils.CreateButton("Load", () => Load());
            
            toolbar.Add(_textField);
            toolbar.Add(_button);
            toolbar.Add(clearButton);
            toolbar.Add(loadButton);
            StyleSheet styleSheet1 =
                (StyleSheet)EditorGUIUtility.Load(
                    "DLSystem/DLSystemToolBar.uss");
            toolbar.styleSheets.Add(styleSheet1);
            rootVisualElement.Add(toolbar);
        }

        private void Clear()
        {
            _graphView.ClearGraph();
        }

        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DLSystem/Graphs", "asset");
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Clear();

        }
        
        private void ResetGraph()
        {
            Clear();

            UpdateFileName(_defaultName);
        }
        
        public static void UpdateFileName(string newFileName)
        {
            _textField.value = newFileName;
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