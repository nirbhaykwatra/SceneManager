using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace SceneManager
{
    // TODO: Automatically unassign types from scenes after their type is deleted.
    ///<summary>
    /// Class containing the Create New Scene Window. Contained within the Scene Manager package.
    /// </summary>
    public class CreateNewSceneWindow : OdinEditorWindow
    {
        
        // Method to open the Create New Scene window. Adds Create New Scene window to the File menu.
        [MenuItem("File/Create New Scene %n", priority = 10)]
        private static void OpenEditor() => GetWindow<CreateNewSceneWindow>();
        
        // I know what you're thinking. But "SceneTypesList" is a static type, why am I caching it to a variable here?
        // Check Line 53.
        private List<string> sceneTypes = SceneUtilities.SceneTypesList;
        
        [ShowInInspector]
        private string sceneName;
        
        // Scene path field, requires an existing path as valid input.
        [FolderPath(RequireExistingPath = true)]
        [ShowInInspector]
        private string scenePath;
        
        [ShowInInspector]
        private bool UseSceneTemplate;
        
        [ShowIf("UseSceneTemplate")]
        [AssetsOnly]
        [ShowInInspector]
        private SceneTemplateAsset sceneTemplate;
        
        // Odin's ValueDropdown attribute doesn't appear to support variables from static classes. Bummer.
        [ValueDropdown("sceneTypes")]
        [ShowInInspector]
        private string sceneType;
        
        // Fields to add or remove scene types
        [PropertyOrder(1000)]
        [Title("Manage Scene Types")]
        [ShowInInspector]
        [InlineButton("CreateNewSceneType", "Create New Scene Type")]
        private string NewSceneType;
        
        [PropertyOrder(1001)]
        [ValueDropdown("sceneTypes")]
        [ShowInInspector]
        [InlineButton("RemoveSceneType", "Remove Scene Type")]
        private string SelectSceneType;
        
        public CreateNewSceneWindow()
        {
            // Import scene types from types.json
            SceneUtilities.ImportSceneTypes();
            
            // Set the sceneType field of the window to the first item in the SceneTypes list.
            // If sceneType is null, the ValueDropdown in the window will be blank by default.
            sceneType = SceneUtilities.GetSceneTypeByIndex(0);
            SelectSceneType = SceneUtilities.GetSceneTypeByIndex(0);
        }
        
        [Button(ButtonSizes.Large, ButtonStyle.Box, Name = "Create Scene")]
        private void CreateNewScene()
        {
            if (sceneType == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a scene type", "OK");
                return;
            }

            if (sceneName == "")
            {
                EditorUtility.DisplayDialog("Error", "Please enter a scene name", "OK");
                return;
            }

            if (scenePath == "")
            {
                EditorUtility.DisplayDialog("Error", "Please enter a scene path", "OK");
                return;
            }
            
            SceneUtilities.CreateNewScene(sceneName, sceneTemplate, scenePath, sceneType, UseSceneTemplate);
        }
        
        private void CreateNewSceneType()
        {
            if (NewSceneType == "")
            {
                EditorUtility.DisplayDialog("Error", "Please enter a scene type", "OK");
                return;
            }

            if (SceneUtilities.DoesSceneTypeExist(NewSceneType))
            {
                EditorUtility.DisplayDialog("Error", "Scene type already exists", "OK");
                return;
            }
            
            SceneUtilities.AddSceneType(NewSceneType);
            SceneUtilities.ImportSceneTypes();
            NewSceneType = "";
            Repaint();
        }
        
        private void RemoveSceneType()
        {
            if (SelectSceneType == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a scene type", "OK");
                return;
            }

            if (!SceneUtilities.DoesSceneTypeExist(SelectSceneType))
            {
                EditorUtility.DisplayDialog("Error", "Scene type does not exist", "OK");
                return;
            }
            
            SceneUtilities.DeleteSceneType(SelectSceneType);
            SceneUtilities.ImportSceneTypes();
            SelectSceneType = SceneUtilities.GetSceneTypeByIndex(0);
            Repaint();
        }
    }
}

