using DLSystem.Enums;
using Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

        // Draw Multi Choice Node 
        protected sealed override void Draw()
        {
            base.Draw();
            Button addChoiceButton = DLSystemUtils.CreateButton("Add Choice");
            mainContainer.Insert(1,addChoiceButton);
            int index = 1;
            foreach (string choice in Choices)
            {
                string portName = "Choice "+index.ToString();
                Port choicePort = this.CreatePort(Orientation.Horizontal, Direction.Output,
                    Port.Capacity.Single, typeof(bool),portName,new string[]{});
                
                index++;
                
                Button deleteChoiceButton = DLSystemUtils.CreateButton("x");
                TextField textField = DLSystemUtils.CreateTextField(choice,
                    new string [] {"choice-text-field"});
                
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
