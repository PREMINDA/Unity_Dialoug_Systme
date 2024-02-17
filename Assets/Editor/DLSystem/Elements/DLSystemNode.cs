using System;
using System.Collections.Generic;
using DLSystem.Enums;
using Editor.DLSystem.Data.Save;
using Editor.DLSystem.Entity;
using Editor.DLSystem.Windows;
using Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DLSystem.Elements
{
    public class DLSystemNode:Node
    {
        public string DialogueNodeName { get; set; }

        public string ID { get; set; }
        public bool IsUnGroup { get; set; }
        public bool IsGoingToDelete { get; set; }
        public Group BelongGroup { get; set;}
        protected List<DLSystemChoiceSaveData> Choices { get; set; }
        private string Text { get; set; }
        public DLSystemType DLSystemType { get; set; }
        protected DLSystemGraphView DLSystemGraphView { get; set; }
        private readonly Color _styleBackgroundColor;
        
        
        protected DLSystemNode()
        { 
            
        }
        protected DLSystemNode(DLSystemGraphView dlSystemGraphView,Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DLSystemGraphView = dlSystemGraphView;
            ColorUtility.TryParseHtmlString( "#1d1d33" , out _styleBackgroundColor );
            IsGoingToDelete = false;
            Initialize(position);
        }

        private void Initialize(Vector2 position)
        {
            DialogueNodeName = "DialogueName";
            Choices = new List<DLSystemChoiceSaveData>();
            Text = "Init Text.";
            SetPosition(new Rect(position,Vector2.zero));
        }

        protected virtual void Draw()
        {
            TextField dialogueNodeName = DLSystemUtils.CreateTextField(DialogueNodeName,
                new string[] { "node-title-input" },onChange:callBack=>{
                    if (IsUnGroup)
                    {
                        DLSystemGraphView.RemoveUngroupNode(this);
                        DialogueNodeName = callBack.newValue;
                        DLSystemGraphView.AddUnGroupNode(this);
                    }
                    else
                    {
                        DLSystemGraphView.RemoveGroupNode(this);
                        DialogueNodeName = callBack.newValue;
                        DLSystemGraphView.AddGroupNode(this,BelongGroup);
                    }
                });
            
            titleContainer.AddToClassList("node-title-base");
            titleContainer.Insert(0,dialogueNodeName);
            Port inputPort = this.CreatePort(Orientation.Horizontal, Direction.Input,
                Port.Capacity.Multi, typeof(bool),"in",new string[]{"input-link-area"});
            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("Ti");
            
            Foldout textFoldout = DLSystemUtils.CreateTextFold("Dialogue Text");
            TextField textField = DLSystemUtils.CreateTextField(Text, new string[] { "dialog-input-field" });
            textFoldout.Add(textField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();


        }

        public void DisconnectAllPorts()
        {
            DisconnectPorts(inputContainer);
            DisconnectPorts(outputContainer);

        }

        private void DisconnectPorts(VisualElement element)
        {
            foreach (var visualElement in element.Children()) if( visualElement != null)
            {
                Port port = (visualElement is Port)?(Port)visualElement :
                    ((DLSystemMultiPortContainerBox)visualElement).ConfigPort;
                
                if (!port.connected)
                {
                    continue;
                }

                DLSystemGraphView.DeleteElements(port.connections);
            }
        }

        public void SetErrorStyle(Color32 color)
        {
            mainContainer.style.backgroundColor = (Color)color;
        }

        public void ReSetErrorStyle()
        {
            
            mainContainer.style.backgroundColor = _styleBackgroundColor;
        }
    }
}