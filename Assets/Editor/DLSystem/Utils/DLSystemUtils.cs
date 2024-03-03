
using System;
using Editor.DLSystem.Data.Save;
using Editor.DLSystem.Elements;
using Editor.DLSystem.Entity;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


namespace Editor.Utils
{
    
    public static class DLSystemUtils
    {
        
        public static TextField CreateTextField(string title,string[] styleClass,string lable = null,bool multiLine = false, EventCallback<ChangeEvent<string>> onChange = null)
        {
            TextField textField = new TextField()
            {
                value = title,
                label = lable
            };
            
            textField.multiline = multiLine;
            
            if (onChange != null)
            {
                textField.RegisterValueChangedCallback(onChange);
            }

            foreach (string s in styleClass)
            {
                textField.AddToClassList(s);
            }
            return textField;
        }

        public static Foldout CreateTextFold(string title)
        {
            return new Foldout() { text = title };
        }

        public static Button CreateButton(string title, Action action = null)
        {
            Button button = new Button(action)
            {
                text = title
            };
            return button;
        }

        public static Port CreatePort(this DLSystemNode node, Orientation orientation, Direction direction,
            Port.Capacity portCapacity, Type type,string portName, string [] classNames)
        {
            Port inputPort = node.InstantiatePort(orientation, direction,
                portCapacity, type);
            inputPort.portName = portName;
            foreach(string s in classNames){
                inputPort.AddToClassList(s);
            }
            return inputPort;
        }
        
        public static VisualElement CreateVisualElement(Port choicePort,string lable = null)
        {
            VisualElement visualElement = new DLSystemMultiPortContainerBox(choicePort);
            DLSystemChoiceSaveData choiceData = (DLSystemChoiceSaveData)choicePort.userData;
            TextField textField = CreateTextField(choiceData.Text,new string [] {"choice-text-field"},lable,
                onChange:callback=>choiceData.Text = callback.newValue);
            visualElement.AddToClassList("choice-box");
            visualElement.Add(choicePort);
            visualElement.Add(textField);
            return visualElement;
        }
    }
}