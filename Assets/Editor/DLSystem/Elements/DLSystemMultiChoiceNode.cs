using DLSystem.Enums;
using Editor.DLSystem.Data.Save;
using Editor.DLSystem.Entity;
using Editor.DLSystem.Windows;
using Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Editor.DLSystem.Elements
{
    public class DLSystemMultiChoiceNode : DLSystemNode
    {
        int _nodeIndex = 1;
        public DLSystemMultiChoiceNode(
            DLSystemGraphView dlSystemGraphView,
            Vector2 position
            ) :base(dlSystemGraphView, position)
        {
            DLSystemType = DLSystemType.MultipleChoice;
            Choices.Add(new DLSystemChoiceSaveData(){Text = "New Choice"});
            Draw();
        }

        // Draw Multi Choice Node 
        protected sealed override void Draw()
        {
            base.Draw();
            Button addChoiceButton = DLSystemUtils.CreateButton("Add Choice", () =>
            {
                DLSystemChoiceSaveData choiceData = new DLSystemChoiceSaveData()
                {
                    Text = "New Choice"
                };

                Choices.Add(choiceData);
                Port choicePort = CreatePort(choiceData,_nodeIndex++);    
                VisualElement visualElement = CreateVisualElement(choicePort);
                outputContainer.Add(visualElement);
                RefreshExpandedState();
            });
            mainContainer.Insert(1,addChoiceButton);
            foreach (DLSystemChoiceSaveData choice in Choices)
            {
                Port choicePort = CreatePort(choice,_nodeIndex++);
                VisualElement visualElement = CreateVisualElement(choicePort);
                outputContainer.Add(visualElement);
                
            }
            RefreshExpandedState();
        }

        private Port CreatePort(DLSystemChoiceSaveData choice,int index)
        {
            string portName = "Choice "+index.ToString();
            Port choicePort = this.CreatePort(Orientation.Horizontal, Direction.Output,
                Port.Capacity.Single, typeof(bool),portName,new string[]{});
            choicePort.userData = choice;
            Button deleteChoiceButton = DLSystemUtils.CreateButton("x",() =>
            {
                _nodeIndex--;
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    DLSystemGraphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choice);
                VisualElement parentContainer = choicePort.parent.parent;
                parentContainer.Remove(choicePort.parent);
                DLSystemGraphView.RemoveElement(choicePort);
            });
            choicePort.Add(deleteChoiceButton);
            return choicePort;
        }

        private VisualElement CreateVisualElement(Port choicePort)
        {
            VisualElement visualElement = new DLSystemMultiPortContainerBox(choicePort);
            DLSystemChoiceSaveData choiceData = (DLSystemChoiceSaveData)choicePort.userData;
            TextField textField = DLSystemUtils.CreateTextField(choiceData.Text,
                new string [] {"choice-text-field"});
            visualElement.AddToClassList("choice-box");
            visualElement.Add(choicePort);
            visualElement.Add(textField);
            return visualElement;
        }

    }
}
