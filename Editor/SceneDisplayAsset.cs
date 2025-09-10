using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace SceneManager
{
    public class SceneAssetDisplay
        {
            // Create sceneUtilities object to access scene helper methods.
            private readonly SceneUtilities sceneUtilities = new SceneUtilities();
            
            // Public scene asset and scene metadata for easy editing.
            [HideInInspector]
            public SceneAsset sceneAsset;
            [HideInInspector]
            private SceneMetadata sceneMetadata;
            
            public static List<string> SceneTypes = new List<string>();
            
            public SceneAssetDisplay(SceneAsset inputSceneAsset)
            {
                // If types.json does not exist, create it with default "Level" value;
                if (!File.Exists("Assets/Editor/types.json"))
                {
                    File.Create("Assets/Editor/types.json");
                    File.WriteAllText("Assets/Editor/types.json", "[\"Level\"]");
                }
                // Parse types.json into a list of scene types, formatted as strings.
                string jsonTypes = File.ReadAllText("Assets/Editor/types.json");
                string[] types = JsonConvert.DeserializeObject<string[]>(jsonTypes);
            
                // Load each scene type into memory using the SceneTypes list.
                foreach (string type in types)
                {
                    SceneTypes.Add(type);
                }
                
                // Get scene metadata of the input scene asset.
                sceneAsset = inputSceneAsset;
                string json = sceneUtilities.GetSceneMetadata(sceneAsset);;
                
                // Deserialize scene metadata
                sceneMetadata = JsonConvert.DeserializeObject<SceneMetadata>(json);
                
                // Set scene type from scene metadata
                Type = sceneMetadata.type;
            }
            
            // Is edit mode active?
            private bool editMode = false;
            
            // Set scene name from metadata.
            public string Name => sceneAsset.name;

            [EnableIf("editMode")]
            [ValueDropdown("SceneTypes")]
            public string Type;
            
            [ShowInInspector]
            public string Path => AssetDatabase.GetAssetPath(sceneAsset);
            
            [HideInInspector]
            [ReadOnly]
            public string GUID => AssetDatabase.AssetPathToGUID(Path);
            
            
            [HideIf("editMode")]
            [Button("Edit Scene Metadata")]
            public void EditSceneMetadata()
            {
                editMode = true;
            }
            
            [ShowIf("editMode")]
            [Button("Save Scene Metadata")]
            public void SaveSceneMetadata()
            {
                string rawScenePath = AssetDatabase.GetAssetPath(sceneAsset);
                string scenePath = rawScenePath.Replace($"{sceneAsset.name}.unity", "");
                
                sceneMetadata.name = sceneAsset.name;
                sceneMetadata.path = scenePath;
                sceneMetadata.guid = AssetDatabase.AssetPathToGUID(rawScenePath);
                sceneMetadata.type = Type;
                
                File.WriteAllText($"{scenePath}/.{sceneAsset.name}.json", JsonConvert.SerializeObject(sceneMetadata));
                
                editMode = false;
            }

            [Button("Refresh Scene Metadata")]
            public void RefreshSceneMetadata()
            {
                string rawScenePath = AssetDatabase.GetAssetPath(sceneAsset);
                string scenePath = rawScenePath.Replace($"{sceneAsset.name}.unity", "");

                sceneMetadata.name = sceneAsset.name;
                sceneMetadata.path = scenePath;
                sceneMetadata.guid = AssetDatabase.AssetPathToGUID(rawScenePath);
                sceneMetadata.type = Type;

                File.WriteAllText($"{scenePath}/.{sceneAsset.name}.json", JsonConvert.SerializeObject(sceneMetadata));

                Type = sceneMetadata.type;
            }
        }
}