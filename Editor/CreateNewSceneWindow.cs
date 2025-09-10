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
            
            // Check if scene types json exists. If not, create it.
            if (!File.Exists("Assets/Editor/types.json"))
            {
                File.WriteAllText("Assets/Editor/types.json", "[\"Level\"]");
            }
            
            // Parse types.json into a list of scene types, formatted as strings.
            string json = File.ReadAllText("Assets/Editor/types.json");
            if (json == "") json = "[\"Level\"]";
            string[] types = JsonConvert.DeserializeObject<string[]>(json);
            
            // Load each scene type into memory using the SceneTypes list.
            foreach (string type in types)
            {
                SceneTypes.Add(type);
            }
            
            // Set the sceneType field of the window to the first item in the SceneTypes list.
            // If sceneType is null, the ValueDropdown in the window will be blank by default.
            sceneType = SceneTypes[0];
            SelectSceneType = SceneTypes[0];
        }
        
        // Method to open the Create New Scene window. Adds Create New Scene window to the File menu.
        [MenuItem("File/Create New Scene %n", priority = 10)]
        private static void OpenEditor() => GetWindow<CreateNewSceneWindow>();
        
        // Create a SceneUtilities object to access scene helper methods.
        private readonly SceneUtilities sceneUtilities = new SceneUtilities();
        
        private static List<string> SceneTypes = new List<string>();
        
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
        
        [ValueDropdown("SceneTypes")]
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

            if (SceneTypes.Contains(NewSceneType))
            {
                EditorUtility.DisplayDialog("Error", "Scene type already exists", "OK");
                return;
            }
            
            SceneTypes.Add(NewSceneType);
            string json = JsonConvert.SerializeObject(SceneTypes);
            if (File.Exists("Assets/Editor/types.json"))
            {
                File.Delete("Assets/Editor/types.json");
            }
            File.WriteAllText("Assets/Editor/types.json", json);
            NewSceneType = "";
            GetWindow(typeof(CreateNewSceneWindow));
        }
        
        [PropertyOrder(1001)]
        [ValueDropdown("SceneTypes")]
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

            if (!SceneTypes.Contains(SelectSceneType))
            {
                EditorUtility.DisplayDialog("Error", "Scene type does not exist", "OK");
                return;
            }
            
            SceneTypes.Remove(SelectSceneType);
            string json = JsonConvert.SerializeObject(SceneTypes);
            if (File.Exists("Assets/Editor/types.json"))
            {
                File.Delete("Assets/Editor/types.json");
            }
            File.WriteAllText("Assets/Editor/types.json", json);
            SelectSceneType = SceneTypes[0];
            GetWindow(typeof(CreateNewSceneWindow));
        }
    }
}

