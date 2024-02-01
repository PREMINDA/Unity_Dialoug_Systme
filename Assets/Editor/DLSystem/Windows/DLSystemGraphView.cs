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
        public DLSystemGraphView()
        {
            AddGridBackground();
            AddStyle();
            AddManipulators();
        }
        
        private DLSystemNode CreateNode(DLSystemType dlSystemType,Vector2 position)
        {
            
            DLSystemNode dlSystemNode = dlSystemType == DLSystemType.SingleChoice?
                new DLSystemSingleChoiceNode(position):
                new DLSystemMultiChoiceNode(position);
            AddElement(dlSystemNode);
            return dlSystemNode;
        }

        private void AddManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateDLSystemNodeContextualMenu(DLSystemType.SingleChoice,"Node Single"));
            this.AddManipulator(CreateDLSystemNodeContextualMenu(DLSystemType.MultipleChoice,"Node Multi"));


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