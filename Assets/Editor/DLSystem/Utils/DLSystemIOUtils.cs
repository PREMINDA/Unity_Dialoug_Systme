using System;
using System.Collections.Generic;
using System.Linq;
using DLSystem.Scripts.Data;
using Editor.DLSystem.Elements;
using Editor.DLSystem.Entity;
using Editor.DLSystem.Windows;
using UnityEditor;
using UnityEngine;
using DLSystem.Scripts.ScriptableObjects;
using Editor.DLSystem.Data.Save;
using SerializableDictionary;

namespace Editor.DLSystem.Utils
{
    public class DLSystemIOUtils
    {
        private static DLSystemGraphView _dlSystemGraphView;
        private static string _graphFileName;
        private static string _containerFolderPath;

        private static Dictionary<string, DLSystemDialogueGroupSO> _createdDialogueGroups;
        private static Dictionary<string, DLSystemDialogueSO> _createdDialogues;
        
        private static List<DLSystemNode> _nodes;
        private static List<DLSystemGroup> _groups;
        

        public static void Initialize(string fileName,DLSystemGraphView dlSystemGraphView)
        {
            _graphFileName = fileName;
            _createdDialogues = new Dictionary<string, DLSystemDialogueSO>();
            _createdDialogueGroups = new Dictionary<string, DLSystemDialogueGroupSO>();
            _containerFolderPath =  $"Assets/DLSystem/Dialogues/{fileName}";
            _dlSystemGraphView = dlSystemGraphView;
            _nodes = new List<DLSystemNode>();
            _groups = new List<DLSystemGroup>();
        }

        #region save data

        public static void Save()
        {
            CreateDefaultFolders();
            GetElementFromGraphView();
            DLSystemGraphSaveDataSO graphData = CreateAsset<DLSystemGraphSaveDataSO>("Assets/Editor/DLSystem/Graphs", $"{_graphFileName}Graph");
            graphData.Initialize(_graphFileName);
            DLSystemContainerSO dialogueContainer = CreateAsset<DLSystemContainerSO>(_containerFolderPath, _graphFileName);
            dialogueContainer.Initialize(_graphFileName);
            SaveGroups(graphData,dialogueContainer);
            SaveNodes(graphData, dialogueContainer);
            
            SaveAsset(graphData);
            SaveAsset(dialogueContainer);

        }
        #endregion

        #region GroupsUtils
        private static void SaveGroups(DLSystemGraphSaveDataSO graphData, DLSystemContainerSO dialogueContainer)
        {
            List<string> groupNames = new List<string>();
            foreach (DLSystemGroup group in _groups)
            {
                SaveGroupsInGraph(group,graphData);
                SaveGroupInContainer(group, dialogueContainer);
                
                groupNames.Add(group.title);
            }
            UpdateOldGroups(groupNames, graphData);
            
        }
        
        private static void UpdateOldGroups(List<string> currentGroupNames, DLSystemGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{_containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }

        private static void SaveGroupInContainer(DLSystemGroup group, DLSystemContainerSO dialogueContainer)
        {
            string groupName = group.title;

            CreateFolder($"{_containerFolderPath}/Groups", groupName);
            CreateFolder($"{_containerFolderPath}/Groups/{groupName}", "Dialogues");

            DLSystemDialogueGroupSO dialogueGroup = CreateAsset<DLSystemDialogueGroupSO>($"{_containerFolderPath}/Groups/{groupName}", groupName);

            dialogueGroup.Initialize(groupName);
            
            _createdDialogueGroups.Add(group.ID, dialogueGroup);

            dialogueContainer.GroupDialogues.Add(dialogueGroup, new List<DLSystemDialogueSO>());

            SaveAsset(dialogueGroup);
        }

        private static void SaveGroupsInGraph(DLSystemGroup group, DLSystemGraphSaveDataSO graphData)
        {
            DLSystemGroupSaveData groupSaveData = new DLSystemGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };
            
            graphData.Groups.Add(groupSaveData);
        }

        #endregion

        #region NodeUtils

        private static void SaveNodes(DLSystemGraphSaveDataSO graphData, DLSystemContainerSO dialogueContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();
            
            
            foreach (DLSystemNode node in _nodes)
            {
                SaveNodeInGraph(node, graphData);
                SaveNodeInContainer(node, dialogueContainer);

                if (node.BelongGroup != null)
                {
                    if (!groupedNodeNames.Contains(node.BelongGroup.name))
                    {
                        List<string> nodeNameList = new List<string> { node.DialogueNodeName };
                        groupedNodeNames.Add(node.BelongGroup.name,nodeNameList);
                        continue;
                    }
                    groupedNodeNames[node.BelongGroup.name].Add(node.DialogueNodeName);
                    continue;
                }

                ungroupedNodeNames.Add(node.DialogueNodeName);
            }
            UpdateDialoguesChoicesConnections();
            
            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);

        }
        
        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DLSystemGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{_containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }
        
        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DLSystemGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{_containerFolderPath}/Global/Dialogues", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }

        private static void SaveNodeInGraph(DLSystemNode node, DLSystemGraphSaveDataSO graphData)
        {
            List<DLSystemChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            DLSystemNodeSaveData nodeData = new DLSystemNodeSaveData()
            {
                ID = node.ID,
                Name = node.DialogueNodeName,
                Choices = choices,
                Text = node.Text,
                GroupId = ((DLSystemGroup)node.BelongGroup)?.ID,
                DialogType = node.DLSystemType,
                Position = node.GetPosition().position
            };

            graphData.Nodes.Add(nodeData);
        }
        
        private static void SaveNodeInContainer(DLSystemNode node, DLSystemContainerSO dialogueContainer)
        {
            DLSystemDialogueSO dialogue;

            if (node.BelongGroup != null)
            {
                dialogue = CreateAsset<DLSystemDialogueSO>($"{_containerFolderPath}/Groups/{node.BelongGroup.title}/Dialogues", node.DialogueNodeName);
                if (!dialogueContainer.GroupDialogues.Contains(_createdDialogueGroups[((DLSystemGroup)node.BelongGroup).ID]))
                {
                    List<DLSystemDialogueSO> list = new List<DLSystemDialogueSO>(){dialogue};
                    dialogueContainer.GroupDialogues.Add(_createdDialogueGroups[((DLSystemGroup)node.BelongGroup).ID], list);
                }
                else
                {
                    dialogueContainer.GroupDialogues[_createdDialogueGroups[((DLSystemGroup)node.BelongGroup).ID]].Add(dialogue);
                }
            }
            else
            {
                dialogue = CreateAsset<DLSystemDialogueSO>($"{_containerFolderPath}/Global/Dialogues", node.DialogueNodeName);

                dialogueContainer.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(
                node.DialogueNodeName,
                node.Text,
                ConvertNodeChoicesToDialogueChoices(node.Choices),
                node.DLSystemType,
                node.IsStartingNode()
            );

            _createdDialogues.Add(node.ID, dialogue);

            SaveAsset(dialogue);
        }
        
        private static List<DLSystemDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DLSystemChoiceSaveData> nodeChoices)
        {
            List<DLSystemDialogueChoiceData> dialogueChoices = new List<DLSystemDialogueChoiceData>();

            foreach (DLSystemChoiceSaveData nodeChoice in nodeChoices)
            {
                DLSystemDialogueChoiceData choiceData = new DLSystemDialogueChoiceData()
                {
                    Text = nodeChoice.Text
                };

                dialogueChoices.Add(choiceData);
            }

            return dialogueChoices;
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

        #region Load Utils

        public static void Load()
        {
            DLSystemGraphSaveDataSO saveDataSo = LoadAsset<DLSystemGraphSaveDataSO>("Assets/Editor/DLSystem/Graphs",_graphFileName);
            
            if (saveDataSo == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Editor/DialogueSystem/Graphs/{_graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!"
                );

                return;
            }
        }

        #endregion

        #region Utils Methods
        
        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        public static void RemoveFolder(string path)
        {
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            FileUtil.DeleteFileOrDirectory($"{path}/");
        }
        
        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (var node in _nodes)
            {
                DLSystemDialogueSO dialogue = _createdDialogues[node.ID];

                for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                {
                    DLSystemChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = _createdDialogues[nodeChoice.NodeID];

                    SaveAsset(dialogue);
                }
            }
        }


        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }
        
        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }
        
        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }
        
        private static List<DLSystemChoiceSaveData> CloneNodeChoices(List<DLSystemChoiceSaveData> nodeChoices)
        {
            List<DLSystemChoiceSaveData> choices = new List<DLSystemChoiceSaveData>();

            foreach (DLSystemChoiceSaveData choice in nodeChoices)
            {
                DLSystemChoiceSaveData choiceData = new DLSystemChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };

                choices.Add(choiceData);
            }

            return choices;
        }

        #endregion
    }
}