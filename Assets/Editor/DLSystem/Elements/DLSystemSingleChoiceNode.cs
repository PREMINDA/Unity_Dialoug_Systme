using System.Collections.Generic;
using DLSystem.Enums;
using Editor.DLSystem.Data.Save;
using Editor.DLSystem.Windows;
using Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.DLSystem.Elements
{
    public class DLSystemSingleChoiceNode : DLSystemNode
    {
        public DLSystemSingleChoiceNode(
            DLSystemGraphView dlSystemGraphView,
            Vector2 position,
            string name,
            List<DLSystemChoiceSaveData> choices
            ):base(dlSystemGraphView,position,name)
        {
            DLSystemType = DLSystemType.SingleChoice;
            if (choices == null)
            {
                Choices.Add(new DLSystemChoiceSaveData(){Text = "New Choice"});
            }
            else
            {
                Choices = choices;
            }

            Draw();
            
        }

        // Draw Single Choice Node
        protected sealed override void Draw()
        {
            base.Draw();
            foreach (DLSystemChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(Orientation.Horizontal, Direction.Output,
                    Port.Capacity.Single, typeof(bool),"Choice",new string[]{});
                choicePort.userData = choice;
                VisualElement visualElement = DLSystemUtils.CreateVisualElement(choicePort);
                outputContainer.Add(visualElement);
            }
            RefreshExpandedState();
        }
    }
}
