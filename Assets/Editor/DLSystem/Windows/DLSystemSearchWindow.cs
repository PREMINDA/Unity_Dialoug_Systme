using System.Collections.Generic;
using DLSystem.Enums;
using Editor.DLSystem.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.DLSystem.Windows
{
    public class DLSystemSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DLSystemGraphView _dlSystemGraphView;

        public void Initialize(DLSystemGraphView dlSystemGraphView)
        {
            _dlSystemGraphView = dlSystemGraphView;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"),1),
                new SearchTreeEntry(new GUIContent("Single Choice"))
                {
                    level = 2,
                    userData = DLSystemType.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Multi Choice")){
                    level = 2,
                    userData = DLSystemType.SingleChoice
                },
                new SearchTreeGroupEntry(new GUIContent("Dialogue Group"),1),
                new SearchTreeEntry(new GUIContent("Single Group"))
                {
                    level = 2,
                    userData = new Group()
                }
            };

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 pos = context.screenMousePosition;
            switch (SearchTreeEntry.userData)
            {
                case DLSystemType.SingleChoice:
                {
                    DLSystemNode node =  _dlSystemGraphView.CreateNode(pos,DLSystemType.SingleChoice);
                    _dlSystemGraphView.AddElement(node);
                    return true;
                }
                case DLSystemType.MultipleChoice:
                {
                    DLSystemNode node = _dlSystemGraphView.CreateNode(pos,DLSystemType.SingleChoice);
                    _dlSystemGraphView.AddElement(node);
                    return true;
                }
                case Group _:
                {
                    Group group = _dlSystemGraphView.CreateGroup(pos);
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }
    }
}