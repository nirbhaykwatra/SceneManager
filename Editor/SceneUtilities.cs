using System;
using System.Collections.Generic;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManager
{
    /// <summary>
    /// Struct containing scene metadata.
    /// </summary>
    struct SceneMetadata
    {
        public string guid { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string type { get; set; }
    }

    /// <summary>
    /// A class containing helper methods for the Scene Manager package.
    /// </summary>
    public class SceneUtilities
    {
        private static readonly List<string> SceneTypes = new List<string>();

        public static List<string> SceneTypesList => SceneTypes;
        
        private string sceneTypesPath = "Assets/Editor/.scenemanager/.scenetypes.json";

        /// <summary>
        /// Creates a metadata .json file for the given scene.
        /// </summary>
        /// <param name="sceneAsset">SceneAsset for which to create metadata.</param>
        /// <param name="type">The scene type.</param>
        public void CreateSceneMetadataIfNotExists(SceneAsset sceneAsset, string type = "Level")
        {
            // Get paths to the scene asset, the scene's parent directory and the scene's metadata file.
            string rawScenePath = AssetDatabase.GetAssetPath(sceneAsset);
            string scenePath = rawScenePath.Replace($"{sceneAsset.name}.unity", "");
            string sceneMetadataPath = scenePath + $".{sceneAsset.name}.json";
            
            // Check if the scene's metadata file exists. If it does, return.
            bool fileExists = File.Exists(sceneMetadataPath);
            bool fileHasValidJSON = true;

            if (!fileExists)
            {
                // If the file does not exist, create it.
                using (StreamWriter sw = File.CreateText(sceneMetadataPath))
                {
                    sw.WriteLine("");
                    sw.Close();
                }
            }

            try
            {
                // Attempt to deserialize JSON file.
                string json = File.ReadAllText(sceneMetadataPath);
                // If file is empty, it does not have valid JSON
                if (json == "") fileHasValidJSON = false; 
                JsonConvert.DeserializeObject(json);
            }
            catch (JsonReaderException exception)
            {
                // If JSON file cannot be parsed, set fileHasValidJSON to false.
                fileHasValidJSON = false;
            }
            
            // If the JSON file exists and is valid, do nothing.
            if (fileExists && fileHasValidJSON)
            {
                return;
            }
            
            // If either the file does not exist or does not contain valid JSON, create a new metadata file with default
            // data.
            
            // Create a default metadata object.
            SceneMetadata metadata = new SceneMetadata();
            metadata.guid = AssetDatabase.AssetPathToGUID(rawScenePath);
            metadata.name = sceneAsset.name;
            metadata.path = scenePath;
            metadata.type = type;
            
            // Serialize the default metadata object into a metadata .json file.
            File.WriteAllText(sceneMetadataPath, JsonConvert.SerializeObject(metadata));
            AssetDatabase.Refresh();
        }
        
        public void CreateSceneTypesMetadataIfNotExists()
        {
            // Set path to types.json.
            string metadataPath = sceneTypesPath;
            
            // Check if the scene's metadata file exists. If it does, return.
            bool fileExists = File.Exists(metadataPath);
            bool fileHasValidJSON = true;

            if (!fileExists)
            {
                // Extract the directory path from the full file path
                string directoryPath = Path.GetDirectoryName(sceneTypesPath);

                // Create all directories in the path if they don't exist
                Directory.CreateDirectory(directoryPath);

                // If the file does not exist, create it.
                using (StreamWriter sw = File.CreateText(metadataPath))
                {
                    sw.WriteLine("");
                    sw.Close();
                }
            }

            try
            {
                // Attempt to deserialize JSON file.
                string json = File.ReadAllText(metadataPath);
                // If file is empty, it does not have valid JSON
                if (json == "") fileHasValidJSON = false; 
                JsonConvert.DeserializeObject(json);
            }
            catch (JsonReaderException exception)
            {
                // If JSON file cannot be parsed, set fileHasValidJSON to false.
                fileHasValidJSON = false;
            }
            
            // If the JSON file exists and is valid, do nothing.
            if (fileExists && fileHasValidJSON)
            {
                return;
            }
            
            // If either the file does not exist or does not contain valid JSON, create a new metadata file with default
            // data.
            string[] metadata = {"Level"};
            
            // Serialize the default metadata object into a metadata .json file.
            File.WriteAllText(metadataPath, JsonConvert.SerializeObject(metadata));
        }
        
        /// <summary>
        /// Create a new scene in the project using the given parameters.
        /// </summary>
        /// <param name="name">The scene name.</param>
        /// <param name="template">The scene template from which to create the scene.</param>
        /// <param name="path">The path where the scene will be created.</param>
        /// <param name="type">The scene type.</param>
        /// <param name="useTemplate">Should the scene be created using a scene template?</param>
        public void CreateNewScene(string name, SceneTemplateAsset template, string path, string type, bool useTemplate)
        {
            // Check if the scene already exists. If it does, display an error message.
            if (AssetDatabase.AssetPathExists(path + "/" + name + ".unity"))
            {
                EditorUtility.DisplayDialog("Error", "Scene already exists", "OK");
            }
            
            if (useTemplate)
            {
                // Check if a scene template was provided. If not, display an error message.
                if (template == null)
                {
                    EditorUtility.DisplayDialog("Error", "Scene template not found", "OK");
                    return;
                }
                
                // Create a new scene using the provided scene template.
                InstantiationResult newScene = SceneTemplateService.Instantiate(template, false, path + "/" + name + ".unity");
                
                // Create metadata for the scene.
                CreateSceneMetadataIfNotExists(newScene.sceneAsset, type);
                
                // Save all assets and refresh the project window.
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                // Create a new scene with default GameObjects (sun and main camera).
                Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                
                // Save the opened scene.
                EditorSceneManager.SaveScene(newScene, path + "/" + name + ".unity");
                
                // Save all assets and refresh the project window.
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                // Retrieve the scene asset.
                SceneAsset newSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path + "/" + name + ".unity");
                
                // Create metadata for the scene.
                CreateSceneMetadataIfNotExists(newSceneAsset, type);
            }
        }
        
        /// <summary>
        /// Returns a scene's metadata file as a string.
        /// </summary>
        /// <param name="sceneAsset">The scene whose metadata is to be retrieved.</param>
        /// <returns></returns>
        public string GetSceneMetadata(SceneAsset sceneAsset)
        {
            // Get paths to the scene asset, the scene's parent directory and the scene's metadata file.
            string rawScenePath = AssetDatabase.GetAssetPath(sceneAsset);
            string scenePath = rawScenePath.Replace($"{sceneAsset.name}.unity", "");
            
            // Check if the scene's metadata file exists. If not, create one.
            CreateSceneMetadataIfNotExists(sceneAsset);
            
            // Read the scene's metadata file and return it as a string.
            return File.ReadAllText($"{scenePath}/.{sceneAsset.name}.json");
        }
        
        /// <summary>
        /// Deletes all scene metadata files which do not have a .unity file with the same name, in the same directory.
        /// This effect is project-wide.
        /// </summary>
        public void CleanSceneMetadata()
        {
            string[] metadataFiles = Directory.GetFiles("Assets", ".*.json", SearchOption.AllDirectories);;
            foreach (string metadata in metadataFiles)
            {
                string parentDir = Path.GetDirectoryName(metadata);
                string fileName = Path.GetFileName(metadata);
                string fileNameWithoutExtension = fileName.Replace(".json", "");
                string sceneName = fileNameWithoutExtension.Substring(1);
                if (File.Exists(metadata) && !File.Exists(parentDir + "/" + sceneName + ".unity"))
                {
                    File.Delete(metadata);
                }
            }
        }

        public void ImportSceneTypes()
        {
            CreateSceneTypesMetadataIfNotExists();
            string typesText = File.ReadAllText(sceneTypesPath);
            string[] types = JsonConvert.DeserializeObject<string[]>(typesText);
            SceneTypes.Clear();
            foreach (string type in types)
            {
                SceneTypes.Add(type);
            }
        }
        
        public void ExportSceneTypes()
        {
            string json = JsonConvert.SerializeObject(SceneTypes);
            if (File.Exists(sceneTypesPath))
            {
                File.Delete(sceneTypesPath);
                File.Delete(sceneTypesPath + ".meta");
            }
            File.WriteAllText(sceneTypesPath, json);
        }
        
        public void DeleteSceneType(string type)
        {
            if (!SceneTypes.Contains(type)) return;
            SceneTypes.Remove(type);
            ExportSceneTypes();
        }
        
        public void AddSceneType(string type)
        {
            if (SceneTypes.Contains(type)) return;
            SceneTypes.Add(type);
            ExportSceneTypes();
        }

        public string GetSceneTypeByIndex(int index)
        {
            if (index < 0 || index >= SceneTypes.Count) return "";
            return SceneTypes[index];
        }

        public bool DoesSceneTypeExist(string type)
        {
            return SceneTypes.Contains(type);
        }
    }
}
