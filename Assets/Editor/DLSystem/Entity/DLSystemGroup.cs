using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.DLSystem.Entity
{
    public class DLSystemGroup : Group
    {
        private Color defaultCorderColor;
        private float defaultBoarderWidth;
        public bool isGroupGoingToDelete { get; set; }
        public string oldTitle { get; set; }
        public DLSystemGroup(string groupTitle, Vector2 position):base()
        {
            isGroupGoingToDelete = false;
            title = oldTitle = groupTitle;
            SetPosition(new Rect(position,Vector2.zero));
            defaultCorderColor = contentContainer.style.borderBottomColor.value;
            defaultBoarderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }
        
        public void ReSetErrorStyle()
        {
            contentContainer.style.borderBottomColor = defaultCorderColor;
            contentContainer.style.borderBottomWidth = defaultBoarderWidth;
        }
    }
}