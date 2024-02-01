using System.Collections.Generic;
using DLSystem.Enums;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DLSystem.Elements
{
    public class DLSystemNode:Node
    {
        public string DialogueNodeName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public DLSystemType DLSystemType { get; set; }

        public DLSystemNode()
        { 

        }
        public DLSystemNode(Vector2 position)
        { 
            Initialize(position);
        }

        private void Initialize(Vector2 position)
        {
            DialogueNodeName = "DialogueName";
            Choices = new List<string>();
            Text = "Init Text.";
            SetPosition(new Rect(position,Vector2.zero));
        }

        protected virtual void Draw()
        {
            TextField dialogueNodeName = new TextField()
            {
                value = DialogueNodeName,
                
            };
            dialogueNodeName.AddToClassList("node-title-input");
            titleContainer.AddToClassList("node-title-base");
            titleContainer.Insert(0,dialogueNodeName);
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,
                Port.Capacity.Multi, typeof(bool));
            
            inputPort.portName = "in";
            inputPort.AddToClassList("input-link-area");
            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("Ti");
            
            Foldout textFoldout = new Foldout()
            {
                text = "Dialogue Text"
            };

            TextField textField = new TextField()
            {
                value = Text,
            };
            textField.AddToClassList("dialog-input-field");
            textFoldout.Add(textField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();


        }
    }
}