using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FlockSpawner : MonoBehaviour
{
    private FileSystemWatcher fileSystemWatcher;
    // Note: this value depends on which platform you are running on, so beware where you're sending images from your external tool.
    // So while running in the Unity Editor, files should be input to <path to project folder>/Assets/InputImages
    // However, running it with the built executable on Windows is <path to executablename_Data folder>/InputImages
    // See https://docs.unity3d.com/ScriptReference/Application-dataPath.html for locations on other platforms.
    private string folderToWatch;

    // TODO this would be better implemented as an Option type, but C# doesn't have one, so roll your own.
    private bool newOrUpdatedFile;
    private string pathToNewOrUpdatedFile;

    // Start is called before the first frame update
    void Start()
    {
        newOrUpdatedFile = false;
        pathToNewOrUpdatedFile = "";

        folderToWatch = Path.Combine(Application.dataPath, "InputImages");
        
        fileSystemWatcher = new FileSystemWatcher(folderToWatch);
        fileSystemWatcher.EnableRaisingEvents = true;
        fileSystemWatcher.Created += new FileSystemEventHandler(FileCreatedOrChanged);
        fileSystemWatcher.Changed += new FileSystemEventHandler(FileCreatedOrChanged);
        fileSystemWatcher.Filter = "*.png";
    }

    // Update is called once per frame
    void Update()
    {
        if( newOrUpdatedFile )
        {
            Sprite sprite = IMG2Sprite.instance.LoadNewSprite(pathToNewOrUpdatedFile);

            GameObject go = new GameObject("FlockManager");
            FlockManager fm = go.AddComponent<FlockManager>();
            fm.boidPrefab = (GameObject)Resources.Load("Prefabs/Boid");
            fm.minSpeed = 0.5f;
            fm.maxSpeed = 5.0f;
            fm.neighbourDistance = 10.0f;
            fm.avoidanceFactor = 1.0f;
            fm.centeringFactor = 0.5f;
            fm.velocityMatchingFactor = 0.0f;
            fm.boidSprite = sprite;

            newOrUpdatedFile = false;
            pathToNewOrUpdatedFile = "";
        }
    }

    private void FileCreatedOrChanged(System.Object sender, FileSystemEventArgs e)
    {
        if(e.FullPath.EndsWith(".png"))
        {
            ProcessFile(e.FullPath);
        }
    }

    private void ProcessFile(String filePath)
    {
        Debug.Log(filePath);
        // Rather than creating the flocking manager here, set a flag to indicate the main thread should do so.
        newOrUpdatedFile = true;
        pathToNewOrUpdatedFile = filePath;
    }
}
