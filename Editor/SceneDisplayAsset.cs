using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace SceneManager
{
    public class SceneAssetDisplay
    {
        public static event Action OnSceneAssetChanged;
        public static event Action OnSceneAssetEdit;
            
        // Create sceneUtilities object to access scene helper methods.
        private SceneUtilities sceneUtilities = new SceneUtilities();

        private List<string> sceneTypes = SceneUtilities.SceneTypesList;
        
        // Public scene asset and scene metadata for easy editing.
        [HideInInspector]
        public SceneAsset sceneAsset;
        [HideInInspector]
        private SceneMetadata sceneMetadata;
        
        // Is edit mode active?
        private bool editMode = false;
        
        // Set scene name from metadata.
        public string Name => sceneAsset.name;
        public SceneAssetDisplay(SceneAsset inputSceneAsset)
        {
            // If types.json does not exist, create it with default "Level" value;
            sceneUtilities.ImportSceneTypes();
            
            // Get scene metadata of the input scene asset.
            sceneAsset = inputSceneAsset;
            string json = sceneUtilities.GetSceneMetadata(sceneAsset);;
            
            // Deserialize scene metadata
            sceneMetadata = JsonConvert.DeserializeObject<SceneMetadata>(json);
            
            // Set scene type from scene metadata
            Type = sceneMetadata.type;
        }
        

        [EnableIf("editMode")]
        [ValueDropdown("sceneTypes")]
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
            sceneUtilities.ImportSceneTypes();
            OnSceneAssetEdit?.Invoke();
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
            
            OnSceneAssetChanged?.Invoke();
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
            
            OnSceneAssetChanged?.Invoke();
            Type = sceneMetadata.type;
        }
    }
}