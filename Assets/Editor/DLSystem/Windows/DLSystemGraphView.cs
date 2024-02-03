using System.Collections.Generic;
using DLSystem.Enums;
using Editor.DLSystem.Elements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DLSystem.Windows
{
    public class DLSystemGraphView : GraphView
    {
        private DLSystemSearchWindow _searchWindow;
        public DLSystemGraphView()
        {
            AddGridBackground();
            AddStyle();
            // AddSearchWindow();
            AddManipulators();
        }

        

        private void AddManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateDLSystemNodeContextualMenu(DLSystemType.SingleChoice,"Node Single"));
            this.AddManipulator(CreateDLSystemNodeContextualMenu(DLSystemType.MultipleChoice,"Node Multi"));
            this.AddManipulator(CrateGroupContextualMenu());
        }

        private IManipulator CrateGroupContextualMenu()
        {
            ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator(
                menuEvent=>menuEvent.menu.AppendAction("Create Group",
                    actionEvent=>AddElement(CreateGroup("Dialog Group",actionEvent.eventInfo.localMousePosition))
                )
            );
            return menuManipulator;
        }

        private IManipulator CreateDLSystemNodeContextualMenu(DLSystemType type,string actionTitle)
        {
            ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator(
                menuEvent=>menuEvent.menu.AppendAction(actionTitle,
                    actionEvent=>AddElement(CreateNode(type,actionEvent.eventInfo.localMousePosition))
                    )
                );
            return menuManipulator;
        }

        public DLSystemNode CreateNode(Vector2 contextPosition,DLSystemType dlSystemType)
        {
            return CreateNode(dlSystemType, contextPosition);
        }

        private DLSystemNode CreateNode(DLSystemType dlSystemType,Vector2 position)
        {
            
            DLSystemNode dlSystemNode = dlSystemType == DLSystemType.SingleChoice?
                new DLSystemSingleChoiceNode(position):
                new DLSystemMultiChoiceNode(position);
            AddElement(dlSystemNode);
            return dlSystemNode;
        }

        public Group CreateGroup(Vector2 position)
        {
            return CreateGroup("Dialog Group", position);
        }

        private Group CreateGroup(string title,Vector2 position)
        {
            Group group = new Group()
            {
                title = title
            };
            group.SetPosition(new Rect(position,Vector2.zero));
            return group;
        }


        private void AddStyle()
        {
            StyleSheet styleSheet1 =
                (StyleSheet)EditorGUIUtility.Load(
                    "Assets/Editor Default Resources/DLSystem/DLSystemGraphViewStyle.uss");
            StyleSheet styleSheet2 =
                (StyleSheet)EditorGUIUtility.Load(
                    "Assets/Editor Default Resources/DLSystem/DLSystemNode.uss");
            styleSheets.Add(styleSheet1);
            styleSheets.Add(styleSheet2);

        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0,gridBackground);
        }
        
        private void AddSearchWindow()
        {
            if (_searchWindow == null)
            {
                _searchWindow = ScriptableObject.CreateInstance<DLSystemSearchWindow>();
                _searchWindow.Initialize(this);
            }

            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatibalePort = new List<Port>();
            foreach (Port port in ports)
            {
                if (startPort == port) {continue;}
                if (startPort.node == port.node) continue;
                if (startPort.direction == port.direction) continue;
                compatibalePort.Add(port);
            }
            return compatibalePort;
        }
    }
}