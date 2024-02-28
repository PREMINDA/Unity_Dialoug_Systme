using System.Collections.Generic;
using System.Linq;
using System;
using DLSystem.Enums;
using Editor.DLSystem.Data.Error;
using Editor.DLSystem.Data.Save;
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
        private readonly SerializableDictionary<string, DLSystemNodeErrorData> _unGroupNode;
        private readonly SerializableDictionary<string, DLSystemGroupErrorData> _groups;
        private readonly SerializableDictionary<Group,SerializableDictionary<string,DLSystemNodeErrorData>> _groupNode;
        public DLSystemGraphView()
        {
            _unGroupNode = new SerializableDictionary<string, DLSystemNodeErrorData>();
            _groupNode = new SerializableDictionary<Group, SerializableDictionary<string, DLSystemNodeErrorData>>();
            _groups = new SerializableDictionary<string, DLSystemGroupErrorData>();
            
            AddGridBackground();
            AddStyle();
            OnElementDelete();
            OnGroupElementsAdd();
            OnGroupElementRemove();
            OnGroupTitleChange();
            OnGraphViewChanged();
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
        
        #region AddGroup

        private void AddGroup(DLSystemGroup dlSystemGroup)
        {
            string groupTitle = dlSystemGroup.title;
            
            if (!_groups.ContainsKey(groupTitle))
            {
                DLSystemGroupErrorData groupErrorData = new DLSystemGroupErrorData();
                groupErrorData.Groups.Add(dlSystemGroup);
                _groups.Add(groupTitle,groupErrorData);
                return;
            }

            List<DLSystemGroup> dlSystemGroups = _groups[groupTitle].Groups;
            
            dlSystemGroups.Add(dlSystemGroup);
            Color32 errorColor = _groups[groupTitle].ErrorData.Color;
            dlSystemGroup.SetErrorStyle(errorColor);

            if (dlSystemGroups.Count == 2)
            {
                dlSystemGroups[0].SetErrorStyle(errorColor);
            }
        }

        #endregion

        #region CreateGroup
        
        private Group CreateGroup(string title,Vector2 position)
        {
            DLSystemGroup group = new DLSystemGroup(title, position);
            AddGroup(group);
            return group;
        }

        #endregion
        
        #region AddStyle

        private void AddStyle()
        {
            StyleSheet styleSheet1 =
                (StyleSheet)EditorGUIUtility.Load(
                    "DLSystem/DLSystemGraphViewStyle.uss");
            StyleSheet styleSheet2 =
                (StyleSheet)EditorGUIUtility.Load(
                    "DLSystem/DLSystemNode.uss");
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

        #region OnElementDelete
        
        private void OnElementDelete()
        {
            deleteSelection = (operationName, askUser) =>
            {
                List<DLSystemNode> dlSystemNodesToDelete = new List<DLSystemNode>();
                List<Group> groupsToDelete = new List<Group>();
                List<Edge> edgesToDelete = new List<Edge>();
        
                foreach (var selectable in selection)
                {
                    var element = (GraphElement)selectable;
                    if (element is DLSystemNode node)
                    {
                        dlSystemNodesToDelete.Add(node);
                        continue;
                    }

                    if (element is Edge edge)
                    {
                        edgesToDelete.Add(edge);
                        continue;
                    }

                    if (element is Group group)
                    {
                        groupsToDelete.Add(group);
                    }
                }
                
                foreach (Group group in groupsToDelete) if (groupsToDelete.Count>0)
                {
                    ((DLSystemGroup)group).IsGroupGoingToDelete = true;
                    if (_groupNode.Contains(group))
                    {
                        var dlSystemNodeErrorDatas = _groupNode[group].Values;
                        int count = dlSystemNodeErrorDatas.Count;
                        for (int k = 0; k < count ; k++) if (count>0)
                        {
                            var dlSystemNodeErrorData = dlSystemNodeErrorDatas.ToList()[0];
                            int nodeCount = dlSystemNodeErrorData.Nodes.Count;
                            for (int i = 0; i<nodeCount ; i++ )
                            {
                                var node = dlSystemNodeErrorData.Nodes[0];
                                node.IsGoingToDelete = true;
                                RemoveGroupNode(node);
                                RemoveElement(node);
                            }
                        }
                    }

                    RemoveGroup((DLSystemGroup)group);
                    RemoveElement(group);
                }
        
                foreach (DLSystemNode node in dlSystemNodesToDelete)if (dlSystemNodesToDelete.Count>0)
                {
                    if (node.IsUnGroup)
                    {
                        node.IsGoingToDelete = true;
                        RemoveUngroupNode(node);
                    }
                    RemoveElement(node);
                }

                if (edgesToDelete.Count > 0)
                {
                    DeleteElements(edgesToDelete);
                }
            };
            
        }
        
        #endregion

        #region OnGroupTitleChange

        private void OnGroupTitleChange()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DLSystemGroup dlSystemGroup = (DLSystemGroup)group;

                RemoveGroup(dlSystemGroup);
                dlSystemGroup.OldTitle = newTitle;
                AddGroup(dlSystemGroup);
            };
        }

        #endregion

        #region RemoveUngroupNode
        
        public void RemoveUngroupNode(DLSystemNode dlSystemNode)
        {
            string nodeName = dlSystemNode.DialogueNodeName;
        
            List<DLSystemNode> nodeErrorData = _unGroupNode[nodeName].Nodes;

            dlSystemNode.ReSetErrorStyle();

            if (dlSystemNode.IsGoingToDelete)
            {
                dlSystemNode.DisconnectAllPorts();
            }
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
                if(((DLSystemGroup)group).IsGroupGoingToDelete)return;
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

        #region OnGraphViewChanged

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        DLSystemNode nextNode = (DLSystemNode) edge.input.node;

                        DLSystemChoiceSaveData choiceData = (DLSystemChoiceSaveData) edge.output.userData;
                        choiceData.NodeID = nextNode.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge) element;

                        DLSystemChoiceSaveData choiceData = (DLSystemChoiceSaveData) edge.output.userData;
                        choiceData.NodeID = "";
                    }
                }

                return changes;
            };
        }

        #endregion

        #region RemoveGroupNode

        public void RemoveGroupNode(DLSystemNode dlSystemNode)
        {
            string nodeName = dlSystemNode.DialogueNodeName;
            Group groupId = dlSystemNode.BelongGroup;
            DLSystemNodeErrorData nodeErrorData = _groupNode[groupId][nodeName];

            dlSystemNode.ReSetErrorStyle();

            if (dlSystemNode.IsGoingToDelete)
            {
                dlSystemNode.DisconnectAllPorts();
            }
            
            if (nodeErrorData.Nodes.Count>=2)
            {
                nodeErrorData.Nodes.Remove(dlSystemNode);
                if (dlSystemNode.IsUnGroup)
                {
                    MoveFromGroupToUnGroup(dlSystemNode);
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
                    MoveFromGroupToUnGroup(dlSystemNode);
                }
                _groupNode[groupId].Remove(nodeName);
            }
        }
        
        #endregion

        private void RemoveGroup(DLSystemGroup dlSystemGroup)
        {
            string nodeName = dlSystemGroup.OldTitle;
        
            List<DLSystemGroup> groupErrorData = _groups[nodeName].Groups;

            dlSystemGroup.ReSetErrorStyle();

            if (groupErrorData.Count>=2)
            {
                groupErrorData.Remove(dlSystemGroup);
                if (groupErrorData.Count == 1)
                {
                    groupErrorData[0].ReSetErrorStyle();
                }
                return;
            }

            if (groupErrorData.Count == 1)
            {
                _groups.Remove(nodeName);
            }
        }

        private void MoveFromGroupToUnGroup(DLSystemNode dlSystemNode)
        {
            dlSystemNode.BelongGroup = null;
            if (!dlSystemNode.IsGoingToDelete)
            {
                AddUnGroupNode(dlSystemNode);
            }
        }

        public void ClearGraph()
        {
            graphElements.ForEach(RemoveElement);
            
            _groups.Clear();
            _groupNode.Clear();
            _unGroupNode.Clear();
            
        }

    }
}