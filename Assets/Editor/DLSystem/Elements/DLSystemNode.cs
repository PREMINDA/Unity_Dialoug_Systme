using System.Collections.Generic;
using DLSystem.Enums;
using Editor.DLSystem.Data.Constant;
using Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DLSystem.Elements
{
    using Elements;
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
            TextField dialogueNodeName = DLSystemUtils.CreateTextField(DialogueNodeName,
                new string[] { "node-title-input" });
            
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
        
        public void SetErrorStyle(Color32 color)
        {
            mainContainer.style.backgroundColor = (Color)color;
        }

        public void ReSetErrorStyle()
        {
            mainContainer.style.backgroundColor = Constant.DEFAULT_NODE_COLOR;
        }
    }
}