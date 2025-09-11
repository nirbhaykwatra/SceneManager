using System.CodeDom;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace SceneManager
{
    public class SceneMetadataPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths, bool didDomainReload)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                if (movedAssets[i].EndsWith(".unity"))
                {
                    string source = Path.GetDirectoryName(movedFromAssetPaths[i]);
                    string destination = Path.GetDirectoryName(movedAssets[i]);

                    string sceneSourcePath = movedFromAssetPaths[i];
                    string sceneDestinationPath = movedAssets[i];

                    string sceneName = Path.GetFileNameWithoutExtension(sceneSourcePath);
                    string metadataName = sceneName;

                    string sceneNewName = Path.GetFileNameWithoutExtension(sceneDestinationPath);
                    string metadataNewName = sceneNewName;

                    if (!File.Exists(Path.Join(destination, metadataName)))
                    {
                        if (metadataName != sceneNewName)
                        {
                            File.Move(Path.Join(source, $".{metadataName}.json"),
                                Path.Join(destination, $".{metadataNewName}.json"));
                        }
                        else
                        {
                            File.Move(Path.Join(source, $".{metadataName}.json"),
                                Path.Join(destination, $".{metadataName}.json"));
                        }
                    }
                }
            }
        }
    }
}