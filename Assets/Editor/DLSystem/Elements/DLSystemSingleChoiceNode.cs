using DLSystem.Enums;
using Editor.DLSystem.Data.Save;
using Editor.DLSystem.Windows;
using Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.DLSystem.Elements
{
    public class DLSystemSingleChoiceNode : DLSystemNode
    {
        public DLSystemSingleChoiceNode(
            DLSystemGraphView dlSystemGraphView,
            Vector2 position,
            string name
            ):base(dlSystemGraphView,position,name)
        {
            DLSystemType = DLSystemType.SingleChoice;
            Choices.Add(new DLSystemChoiceSaveData(){Text = "New choice1"});
            Draw();
            
        }

        // Draw Single Choice Node
        protected sealed override void Draw()
        {
            base.Draw();
            foreach (DLSystemChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(Orientation.Horizontal, Direction.Output,
                    Port.Capacity.Single, typeof(bool),choice.Text,new string[]{});
                outputContainer.Add(choicePort);
            }
            RefreshExpandedState();
        }
    }
}
