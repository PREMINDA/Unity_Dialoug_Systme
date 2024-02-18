using System.Collections.Generic;
using Editor.DLSystem.Elements;
using Editor.DLSystem.Entity;
using Editor.DLSystem.Windows;
using UnityEditor;
using UnityEngine;

namespace Editor.DLSystem.Utils
{
    public class DLSystemIOUtils
    {
        private static DLSystemGraphView _dlSystemGraphView;
        private static string _graphFileName;
        private static string _containerFolderPath;

        private static List<DLSystemNode> _nodes;
        private static List<DLSystemGroup> _groups;
        

        public static void Initialize(string fileName,DLSystemGraphView dlSystemGraphView)
        {
            _graphFileName = fileName;
            _containerFolderPath =  $"Assets/DLSystem/Dialogues/{fileName}";
            _dlSystemGraphView = dlSystemGraphView;
            _nodes = new List<DLSystemNode>();
            _groups = new List<DLSystemGroup>();
        }

        #region save data

        public static void Save()
        {
            CreateDefaultFolders();
        }
        
        #endregion
        
        #region Folder Creation
        private static void CreateDefaultFolders()
        {
            CreateFolder("Assets/Editor/DLSystem", "Graphs");

            CreateFolder("Assets", "DLSystem");
            CreateFolder("Assets/DLSystem", "Dialogues");

            CreateFolder("Assets/DLSystem/Dialogues", _graphFileName);
            CreateFolder(_containerFolderPath, "Global");
            CreateFolder(_containerFolderPath, "Groups");
            CreateFolder($"{_containerFolderPath}/Global", "Dialogues");
        }
        
        private static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }

        #endregion

        #region Fetch Methods

        private static void GetElementFromGraphView()
        {
            _dlSystemGraphView.graphElements.ForEach(element =>
            {
                if (element is DLSystemNode node)
                {
                    _nodes.Add(node);
                    return;
                }
                
                if (element is DLSystemGroup group)
                {
                    _groups.Add(group);
                    return;
                }

            });
        }

        #endregion

        #region Utils Methods

        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T  asset = ScriptableObject.CreateInstance<T>();;
            
            return asset;
        }

        #endregion
    }
}