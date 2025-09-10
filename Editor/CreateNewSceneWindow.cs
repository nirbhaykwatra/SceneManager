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
        public CreateNewSceneWindow()
        {
            // Import scene types from types.json
            sceneUtilities.ImportSceneTypes();
            
            // Set the sceneType field of the window to the first item in the SceneTypes list.
            // If sceneType is null, the ValueDropdown in the window will be blank by default.
            sceneType = sceneUtilities.GetSceneTypeByIndex(0);
            SelectSceneType = sceneUtilities.GetSceneTypeByIndex(0);
        }
        
        // Method to open the Create New Scene window. Adds Create New Scene window to the File menu.
        [MenuItem("File/Create New Scene %n", priority = 10)]
        private static void OpenEditor() => GetWindow<CreateNewSceneWindow>();
        
        // Create a SceneUtilities object to access scene helper methods.
        private readonly SceneUtilities sceneUtilities = new SceneUtilities();
        
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
        
        [ValueDropdown("sceneTypes")]
        [ShowInInspector]
        private string sceneType;
        
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
            
            sceneUtilities.CreateNewScene(sceneName, sceneTemplate, scenePath, sceneType, UseSceneTemplate);
        }
        
        // Fields to add or remove scene types
        [PropertyOrder(1000)]
        [Title("Manage Scene Types")]
        [ShowInInspector]
        [InlineButton("CreateNewSceneType", "Create New Scene Type")]
        private string NewSceneType;
        private void CreateNewSceneType()
        {
            if (NewSceneType == "")
            {
                EditorUtility.DisplayDialog("Error", "Please enter a scene type", "OK");
                return;
            }

            if (sceneUtilities.DoesSceneTypeExist(NewSceneType))
            {
                EditorUtility.DisplayDialog("Error", "Scene type already exists", "OK");
                return;
            }
            
            sceneUtilities.AddSceneType(NewSceneType);
            sceneUtilities.ImportSceneTypes();
            NewSceneType = "";
            Repaint();
        }
        
        [PropertyOrder(1001)]
        [ValueDropdown("sceneTypes")]
        [ShowInInspector]
        [InlineButton("RemoveSceneType", "Remove Scene Type")]
        private string SelectSceneType;
        private void RemoveSceneType()
        {
            if (SelectSceneType == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a scene type", "OK");
                return;
            }

            if (!sceneUtilities.DoesSceneTypeExist(SelectSceneType))
            {
                EditorUtility.DisplayDialog("Error", "Scene type does not exist", "OK");
                return;
            }
            
            sceneUtilities.DeleteSceneType(SelectSceneType);
            sceneUtilities.ImportSceneTypes();
            SelectSceneType = sceneUtilities.GetSceneTypeByIndex(0);
            Repaint();
        }
    }
}

