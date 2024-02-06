using System;
using System.Collections.Generic;
using DLSystem.Enums;
using Editor.DLSystem.Data.Error;
using Editor.DLSystem.Elements;
using Editor.DLSystem.Entity;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace Editor.DLSystem.Windows
{
    using SerializableDictionary;
    public class DLSystemGraphView : GraphView
    {
        private SerializableDictionary<String, DLSystemNodeErrorData> _unGroupNode;
        private SerializableDictionary<Group,SerializableDictionary<string,DLSystemNodeErrorData>> _groupNode;
        public DLSystemGraphView()
        {
            _unGroupNode = new SerializableDictionary<string, DLSystemNodeErrorData>();
            _groupNode = new SerializableDictionary<Group, SerializableDictionary<string, DLSystemNodeErrorData>>();
            
            AddGridBackground();
            AddStyle();
            OnElementDelete();
            OnGroupElementsAdd();
            OnGroupElementRemove();
            // AddSearchWindow();
            AddManipulators();
        }



        #region ManipulatorSetup

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

        #endregion

        #region CreateNode

        public DLSystemNode CreateNode(Vector2 contextPosition,DLSystemType dlSystemType)
        {
            return CreateNode(dlSystemType, contextPosition);
        }

        private DLSystemNode CreateNode(DLSystemType dlSystemType,Vector2 position)
        {
            
            DLSystemNode dlSystemNode = dlSystemType == DLSystemType.SingleChoice?
                new DLSystemSingleChoiceNode(this,position):
                new DLSystemMultiChoiceNode(this,position);
            AddElement(dlSystemNode);

            AddUnGroupNode(dlSystemNode);
            
            return dlSystemNode;
        }

        #endregion

        #region AddUnGroupNode

        public void AddUnGroupNode(DLSystemNode dlSystemNode)
        {
            string nodeName = dlSystemNode.DialogueNodeName;
            dlSystemNode.IsUnGroup = true;
            dlSystemNode.BelongGroup = null;
            if (!_unGroupNode.ContainsKey(nodeName))
            {
                DLSystemNodeErrorData nodeErrorData = new DLSystemNodeErrorData();
                nodeErrorData.Nodes.Add(dlSystemNode);
                _unGroupNode.Add(nodeName,nodeErrorData);
                return;
            }

            List<DLSystemNode> listNodes = _unGroupNode[nodeName].Nodes;
            
            listNodes.Add(dlSystemNode);
            Color32 errorColor = _unGroupNode[nodeName].ErrorData.Color;
            dlSystemNode.SetErrorStyle(errorColor);

            if (listNodes.Count == 2)
            {
                listNodes[0].SetErrorStyle(errorColor);
            }
        }

        #endregion

        #region CreateGroup

        public Group CreateGroup(Vector2 position)
        {
            return CreateGroup("Dialog Group", position);
        }

        private Group CreateGroup(string title,Vector2 position)
        {
            DLSystemGroup group = new DLSystemGroup()
            {
                title = title,
            };
            
            group.SetPosition(new Rect(position,Vector2.zero));
            return group;
        }

        #endregion
        
        #region AddStyle

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

        #endregion

        #region AddGrideBG

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0,gridBackground);
        }
        
        #endregion

        #region FindingCompatibalePorts

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePort = new List<Port>();
            foreach (Port port in ports)
            {
                if (startPort == port) {continue;}
                if (startPort.node == port.node) continue;
                if (startPort.direction == port.direction) continue;
                compatiblePort.Add(port);
            }
            return compatiblePort;
        }

        #endregion

        #region RemoveElement
        
        private void OnElementDelete()
        {
            deleteSelection = (operationName, askUser) =>
            {
                List<DLSystemNode> dlSystemNodesToDelete = new List<DLSystemNode>();
                List<Group> groupsToDelete = new List<Group>();
        
                foreach (var selectable in selection)
                {
                    var element = (GraphElement)selectable;
                    if (element is DLSystemNode node)
                    {
                        dlSystemNodesToDelete.Add(node);
                    }
                    
                    if (element is Group group)
                    {
                        groupsToDelete.Add(group);
                    }
                }
                
                foreach (Group group in groupsToDelete) if (groupsToDelete.Count>0)
                {
                    
                    RemoveElement(group);
                }
        
                foreach (DLSystemNode node in dlSystemNodesToDelete)if (dlSystemNodesToDelete.Count>0)
                {
                    if (node.IsUnGroup)
                    {
                        RemoveUngroupNode(node);
                    }

                    node.IsGoingToDelete = true;
                    RemoveElement(node);
                }
            };
            
        }
        
        public void RemoveUngroupNode(DLSystemNode dlSystemNode)
        {
            string nodeName = dlSystemNode.DialogueNodeName;
        
            List<DLSystemNode> nodeErrorData = _unGroupNode[nodeName].Nodes;

            dlSystemNode.ReSetErrorStyle();

            if (nodeErrorData.Count>=2)
            {
                nodeErrorData.Remove(dlSystemNode);
                if (nodeErrorData.Count == 1)
                {
                    nodeErrorData[0].ReSetErrorStyle();
                }
                return;
            }

            if (nodeErrorData.Count == 1)
            {
                _unGroupNode.Remove(dlSystemNode.DialogueNodeName);
            }
        }
        
        #endregion

        #region OnGroupElementAdd

        private void OnGroupElementsAdd()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DLSystemNode))
                    {
                        continue;
                    }
                    DLSystemNode dlSystemNode = (DLSystemNode)element;
                    dlSystemNode.BelongGroup = group;
                    RemoveUngroupNode(dlSystemNode);
                    AddGroupNode(dlSystemNode,group);
                }
            };
        }
        
        #endregion

        #region OnGroupElementRemvoe

        private void OnGroupElementRemove()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DLSystemNode))
                    {
                        continue;
                    }
                    DLSystemNode dlSystemNode = (DLSystemNode)element;
                    dlSystemNode.IsUnGroup = true;
                    RemoveGroupNode(dlSystemNode);
                }
            };
        }

        #endregion

        #region AddGroupNode
        public void AddGroupNode(DLSystemNode dlSystemNode, Group groupId)
        {
            string nodeName = dlSystemNode.DialogueNodeName;
            dlSystemNode.IsUnGroup = false;
            if (!_groupNode.Contains(groupId))
            {
                _groupNode.Add(groupId,new SerializableDictionary<string, DLSystemNodeErrorData>());
            }

            if (!_groupNode[groupId].ContainsKey(nodeName))
            {
                DLSystemNodeErrorData nodeErrorData = new DLSystemNodeErrorData();
                nodeErrorData.Nodes.Add(dlSystemNode);
                _groupNode[groupId].Add(nodeName,nodeErrorData);
            }
            else
            {
                _groupNode[groupId][nodeName].Nodes.Add(dlSystemNode);
                Color32 color = _groupNode[groupId][nodeName].ErrorData.Color;
                dlSystemNode.SetErrorStyle(color);
                if (_groupNode[groupId][nodeName].Nodes.Count == 2)
                {
                    _groupNode[groupId][nodeName].Nodes[0].SetErrorStyle(color);
                }

            }
        }
        #endregion

        #region RemoveGroupNode

        public void RemoveGroupNode(DLSystemNode dlSystemNode)
        {
            string nodeName = dlSystemNode.DialogueNodeName;
            Group groupId = dlSystemNode.BelongGroup;
            DLSystemNodeErrorData nodeErrorData = _groupNode[groupId][nodeName];

            dlSystemNode.ReSetErrorStyle();

            if (nodeErrorData.Nodes.Count>=2)
            {
                nodeErrorData.Nodes.Remove(dlSystemNode);
                if (dlSystemNode.IsUnGroup)
                {
                    RemoveFormGroupDictionary(dlSystemNode);
                }
                if (nodeErrorData.Nodes.Count == 1)
                {
                    nodeErrorData.Nodes[0].ReSetErrorStyle();
                }
                return;
            }

            if (nodeErrorData.Nodes.Count == 1)
            {
                if (dlSystemNode.IsUnGroup)
                {
                    RemoveFormGroupDictionary(dlSystemNode);
                }
                _groupNode[groupId].Remove(nodeName);
            }
        }
        
        #endregion

        private void RemoveFormGroupDictionary(DLSystemNode dlSystemNode)
        {
            dlSystemNode.BelongGroup = null;
            if (!dlSystemNode.IsGoingToDelete)
            {
                AddUnGroupNode(dlSystemNode);
            }
        }

    }
}