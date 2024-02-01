using DLSystem.Enums;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Editor.DLSystem.Elements
{
    public class DLSystemMultiChoiceNode : DLSystemNode
    {
        public DLSystemMultiChoiceNode(Vector2 position):base(position)
        {
            DLSystemType = DLSystemType.MultipleChoice;
            Choices.Add("New choice1");
            Choices.Add("New choice2");
            Draw();
        }

        protected sealed override void Draw()
        {
            base.Draw();

            Button addChoiceButton = new Button()
            {
                text = "Add Choice",
            };
            
        
            mainContainer.Insert(1,addChoiceButton);
            int index = 1;
            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal,
                    Direction.Output, Port.Capacity.Single, typeof(bool)
                );
                
                choicePort.portName = "Choice "+index.ToString();
                index++;
                Button deleteChoiceButton = new Button()
                {
                    text = "x"
                };

                TextField textField = new TextField()
                {
                    value = choice
                };
                
                textField.AddToClassList("choice-text-field");
                choicePort.Add(deleteChoiceButton);
                VisualElement visualElement = new Box();
                visualElement.AddToClassList("choice-box");
                visualElement.Add(choicePort);
                visualElement.Add(textField);
                outputContainer.Add(visualElement);
                
            }
            RefreshExpandedState();
        }
    
    }
}
