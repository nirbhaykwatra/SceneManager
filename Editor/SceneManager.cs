using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SceneManager
{
    /// <summary>
    /// Class containing the main Scene Manager window.
    /// </summary>
    public class SceneManagerWindow : OdinMenuEditorWindow
    {
        // Create a SceneUtilities object to access scene helper methods.
        private readonly SceneUtilities sceneUtilities = new SceneUtilities();

        private bool showByPath = false;

        public SceneManagerWindow()
        {
            // Clean unused scene metadata
            sceneUtilities.CleanSceneMetadata();
            
            // Check if scene types json exists. If not, create it.
            sceneUtilities.CreateSceneTypesMetadataIfNotExists();
        }
        
        // Method to open the Scene Manager window. Adds Scene Manager to the File menu.
        [MenuItem("File/Scene Manager %#n", priority = 10)]
        private static void OpenEditor() => GetWindow<SceneManagerWindow>();

        protected override void OnEnable()
        {
            base.OnEnable();
            titleContent = new GUIContent("Scene Manager");
            AssetDatabase.Refresh();
            SceneAssetDisplay.OnSceneAssetChanged += ForceMenuTreeRebuild;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SceneAssetDisplay.OnSceneAssetChanged -= ForceMenuTreeRebuild;
        }

        protected override void OnImGUI()
        {
            // Add top row buttons to the Scene Manager window.
            Rect rect = GUILayoutUtility.GetRect(10, 100, 10, 50, GUILayout.ExpandWidth(true));
            Rect btnRect1 = rect.Split(0, 4);
            Rect btnRect2 = rect.Split(1, 4);
            Rect btnRect3 = rect.Split(2, 4);
            Rect btnRect4 = rect.Split(3, 4);
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.alignment = TextAnchor.MiddleCenter;
            style.stretchHeight = true;
            style.stretchWidth = true;
            if (GUI.Button(btnRect1,"Clean Project Metadata", style))
            {
                sceneUtilities.CleanSceneMetadata();
                sceneUtilities.ImportSceneTypes();
                ForceMenuTreeRebuild();
            }
            
            if (GUI.Button(btnRect2,"Delete Selected Scene", style))
            {
                // Get selected scene path.
                SceneAssetDisplay selection = MenuTree.Selection.SelectedValue as SceneAssetDisplay;
                string rawScenePath = AssetDatabase.GetAssetPath(selection.sceneAsset);
                string scenePath = rawScenePath.Replace($"{selection.sceneAsset.name}.unity", "");
                    
                // Delete scene metadata
                if (File.Exists(scenePath + $".{selection.Name}.json"))
                {
                    File.Delete(scenePath + $".{selection.Name}.json");  
                }
                // Delete scene .meta file
                if (File.Exists(scenePath + $"{selection.Name}.meta"))
                {
                    File.Delete(scenePath + $"{selection.Name}.meta");  
                }
                // Delete scene folder
                if (Directory.Exists(scenePath + selection.Name))
                {
                    Directory.Delete($"{scenePath}/{selection.Name}", true);   
                }
                    
                // Replace scene GUI with no GUI as scene is no longer available.
                MenuTree.Add(selection.sceneAsset.name, null);
                // Delete scene asset file.
                AssetDatabase.DeleteAsset(rawScenePath);
                // Refresh asset database.
                AssetDatabase.Refresh();
                ForceMenuTreeRebuild();
            }

            if (GUI.Button(btnRect3, "Open Selected Scene", style))
            {
                SceneAssetDisplay selection = MenuTree.Selection.SelectedValue as SceneAssetDisplay;
                string rawScenePath = AssetDatabase.GetAssetPath(selection.sceneAsset);
                Scene activeScene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(rawScenePath);
            }

            if (GUI.Button(btnRect4, "Create New Scene", style))
            {
                GetWindow(typeof(CreateNewSceneWindow));
            }
            base.OnImGUI();
        }
        
        protected override void OnBeginDrawEditors()
        {
            // Remove empty items from menu tree and draw a menu search bar
            MenuTree.CollapseEmptyItems();
            MenuTree.DrawSearchToolbar();

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                // Create spacing in front of toolbar buttons
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Refresh"))
                {
                    // Refresh asset database
                    AssetDatabase.Refresh();
                    ForceMenuTreeRebuild();
                }
                
                if (SirenixEditorGUI.ToolbarButton("Show In Project"))
                {
                    // Get selected scene.
                    SceneAssetDisplay selection = MenuTree.Selection.SelectedValue as SceneAssetDisplay;
                    string rawScenePath = AssetDatabase.GetAssetPath(selection.sceneAsset);
                    
                    // Ping scene in project window.
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(rawScenePath));
                }
                
                // Show By Path toggle
                bool showByPathButton = SirenixEditorGUI.ToolbarToggle(showByPath, "Show By Path");
                if (showByPathButton != showByPath)
                {
                    showByPath = showByPathButton;
                    ForceMenuTreeRebuild();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree();
            
            AddAllSceneAssetsToMenu(tree);
            
            return tree;
        }
        
        // Function to add all detected scene assets to an Odin Menu Tree.
        private void AddAllSceneAssetsToMenu(OdinMenuTree tree)
        {
            // Get all scene assets in the project as GUIDs.
            string[] sceneAssetGUIDs = AssetDatabase.FindAssets("t:SceneAsset a:assets");
            
            foreach (string sceneAssetGUID in sceneAssetGUIDs)
            {
                // Load scene asset from GUID.
                string assetPath = AssetDatabase.GUIDToAssetPath(sceneAssetGUID);
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                
                // If scene asset was not found, move on to the next GUID.
                if (sceneAsset == null) continue;
                
                // Get scene metadata as a json string.
                string json = sceneUtilities.GetSceneMetadata(sceneAsset);
                
                // Deserialize json into a SceneMetadata object
                SceneMetadata sceneMetadata = JsonConvert.DeserializeObject<SceneMetadata>(json);
                
                // If Show By Path is active, order scenes under their respective folders in the menu tree.
                // If not, sort scenes by type as a default.
                if (showByPath)
                {
                    tree.AddAssetAtPath(sceneMetadata.path + sceneAsset.name, assetPath);
                    tree.Add(sceneMetadata.path + sceneAsset.name, new SceneAssetDisplay(sceneAsset));
                }
                else
                {
                    tree.AddAssetAtPath(sceneMetadata.type +"/"+ sceneAsset.name, assetPath);
                    tree.Add(sceneMetadata.type +"/"+ sceneAsset.name, new SceneAssetDisplay(sceneAsset));
                }
            }
        }
    }
}
