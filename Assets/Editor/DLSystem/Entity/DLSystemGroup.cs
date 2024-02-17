using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.DLSystem.Entity
{
    public sealed class DLSystemGroup : Group
    {
        
        private readonly Color _defaultCorderColor;
        private readonly float _defaultBoarderWidth;
        public bool IsGroupGoingToDelete { get; set; }
        public string ID { get; set; }
        public string OldTitle { get; set; }
        public DLSystemGroup(string groupTitle, Vector2 position):base()
        {
            IsGroupGoingToDelete = false;
            ID = Guid.NewGuid().ToString();
            title = OldTitle = groupTitle;
            SetPosition(new Rect(position,Vector2.zero));
            _defaultCorderColor = contentContainer.style.borderBottomColor.value;
            _defaultBoarderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public sealed override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
        }

        public sealed override string title
        {
            get { return base.title; }
            set { base.title = value; }
        }

        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }
        
        public void ReSetErrorStyle()
        {
            contentContainer.style.borderBottomColor = _defaultCorderColor;
            contentContainer.style.borderBottomWidth = _defaultBoarderWidth;
        }
    }
}