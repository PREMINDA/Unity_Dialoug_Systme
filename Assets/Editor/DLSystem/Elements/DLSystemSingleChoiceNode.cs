using DLSystem.Enums;
using Editor.Utils;
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

        // Draw Single Choice Node
        protected sealed override void Draw()
        {
            base.Draw();
            foreach (string choice in Choices)
            {
                Port choicePort = this.CreatePort(Orientation.Horizontal, Direction.Output,
                    Port.Capacity.Single, typeof(bool),choice,new string[]{});
                outputContainer.Add(choicePort);
            }
            RefreshExpandedState();
        }
    }
}
