using DLSystem.Enums;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.DLSystem.Elements
{
    public class DLSystemSingleChoiceNode : DLSystemNode
    {
        public DLSystemSingleChoiceNode(Vector2 position):base(position)
        {
            DLSystemType = DLSystemType.SingleChoice;
            Choices.Add("New choice1");
            Draw();
            
        }


        protected sealed override void Draw()
        {
            base.Draw();
            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal,
                    Direction.Output, Port.Capacity.Single, typeof(bool)
                );
                choicePort.portName = choice;
                Debug.Log(choice);
                outputContainer.Add(choicePort);
            }
            RefreshExpandedState();
        }
    }
}
