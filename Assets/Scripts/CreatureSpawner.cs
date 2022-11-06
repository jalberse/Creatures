using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

enum Creature
{
    Boid,
    Snake
}

public class CreatureSpawner : MonoBehaviour
{
    private FileSystemWatcher fileSystemWatcher;
    // Note: this value depends on which platform you are running on, so beware where you're sending images from your external tool.
    // So while running in the Unity Editor, files should be input to <path to project folder>/Assets/InputImages
    // However, running it with the built executable on Windows is <path to executablename_Data folder>/InputImages
    // See https://docs.unity3d.com/ScriptReference/Application-dataPath.html for locations on other platforms.
    private string folderToWatch;

    private Queue<string> newOrUpdatedFiles;

    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        newOrUpdatedFiles = new Queue<string>();

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
        while (newOrUpdatedFiles.Count > 0)
        {
            string newFilePath = newOrUpdatedFiles.Dequeue();
            Texture2D texture = IMG2Sprite.instance.LoadTexture(newFilePath);

            GameObject go = new GameObject("FlockManager");
            FlockManager fm = go.AddComponent<FlockManager>();
            fm.boidPrefab = (GameObject)Resources.Load("Prefabs/Boid");
            fm.boidCardPrefab = (GameObject)Resources.Load("Prefabs/BoidCard");
            
            fm.maxSpeed = 5.0f;
            fm.neighbourDistance = 10.0f;
            fm.avoidanceFactor = 1.0f;
            fm.centeringFactor = 0.5f;
            fm.velocityMatchingFactor = 0.0f;
            fm.boidTexture = texture;
            var creatureTypes = Enum.GetValues(typeof(Creature));
            var randomIndex = this.random.Next(creatureTypes.Length);
            var creatureType = (Creature)creatureTypes.GetValue(randomIndex);
            switch (creatureType) {
                case Creature.Boid:
                    fm.minSpeed = 0.5f;
                    fm.tailLength = 0;
                    break;
                case Creature.Snake:
                    fm.minSpeed = 3.0f;
                    fm.tailLength = 20;
                    break;
            }
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
        newOrUpdatedFiles.Enqueue(filePath);
    }
}
