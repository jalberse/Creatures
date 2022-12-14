using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Collections.Concurrent;

enum Creature
{
    Boid,
    Snake,
    Tree,
}

public class CreatureSpawner : MonoBehaviour
{
    public Vector3 treeSpawnLimitsMin = new Vector3(-7, 1.8f, 5);
    public Vector3 treeSpawnLimitsMax = new Vector3(7, 1.9f, 30);
    private FileSystemWatcher fileSystemWatcher;
    // Note: this value depends on which platform you are running on, so beware where you're sending images from your external tool.
    // So while running in the Unity Editor, files should be input to <path to project folder>/Assets/InputImages
    // However, running it with the built executable on Windows is <path to executablename_Data folder>/InputImages
    // See https://docs.unity3d.com/ScriptReference/Application-dataPath.html for locations on other platforms.
    private string folderToWatch;
    private ConcurrentQueue<string> newOrUpdatedFiles;
    private ConcurrentQueue<string> newOrUpdatedGifFiles;

    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        newOrUpdatedFiles = new ConcurrentQueue<string>();
        newOrUpdatedGifFiles = new ConcurrentQueue<string>();

        folderToWatch = Path.Combine(Application.dataPath, "InputImages");
        
        fileSystemWatcher = new FileSystemWatcher(folderToWatch);
        fileSystemWatcher.EnableRaisingEvents = true;
        fileSystemWatcher.Created += new FileSystemEventHandler(FileCreatedOrChanged);
        fileSystemWatcher.Changed += new FileSystemEventHandler(FileCreatedOrChanged);
        fileSystemWatcher.Filter = "*.png";
        fileSystemWatcher.Filter = "*.gif";
    }

    // Update is called once per frame
    void Update()
    {
        while (newOrUpdatedGifFiles.Count > 0)
        {
            Creature[] creatureTypes = new Creature[] { Creature.Boid };
            var randomIndex = this.random.Next(creatureTypes.Length);
            var creatureType = (Creature)creatureTypes.GetValue(randomIndex);

            string newFilePath = null;
            newOrUpdatedGifFiles.TryDequeue(out newFilePath);

            List<Texture2D> textures = IMGLoader.instance.LoadGif(newFilePath);

            switch (creatureType)
            {
                case Creature.Boid:
                    GameObject go = new GameObject("FlockManager");
                    FlockManager fm = go.AddComponent<FlockManager>();
                    fm.boidPrefab = (GameObject)Resources.Load("Prefabs/Boid");
                    fm.boidCardPrefab = (GameObject)Resources.Load("Prefabs/BoidCard");

                    fm.maxSpeed = 5.0f;
                    fm.neighbourDistance = 10.0f;
                    fm.avoidanceFactor = 1.0f;
                    fm.centeringFactor = 0.5f;
                    fm.velocityMatchingFactor = 0.0f;
                    fm.minSpeed = 0.5f;
                    fm.tailLength = 0;
                    fm.boidTextures = textures;
                    break;
            }
        }

        while (newOrUpdatedFiles.Count > 0)
        {
            Creature[] creatureTypes = new Creature[] { Creature.Snake, Creature.Tree };
            var randomIndex = this.random.Next(creatureTypes.Length);
            var creatureType = (Creature)creatureTypes.GetValue(randomIndex);

            string newFilePath = null;
            newOrUpdatedFiles.TryDequeue(out newFilePath);
            Texture2D texture = IMGLoader.instance.LoadTexture(newFilePath);

            switch (creatureType) {
                case Creature.Tree:
                    GameObject treeGo = new GameObject("Tree");
                    TreeCreature treeCreature = treeGo.AddComponent<TreeCreature>();
                    treeCreature.boidCardPrefab = (GameObject)Resources.Load("Prefabs/BoidCard");
                    treeCreature.cardTexture = texture;
                    Vector3 pos = this.transform.position + new Vector3
                        (
                        UnityEngine.Random.Range(treeSpawnLimitsMin.x, treeSpawnLimitsMax.x),
                        UnityEngine.Random.Range(treeSpawnLimitsMin.y, treeSpawnLimitsMax.y),
                        UnityEngine.Random.Range(treeSpawnLimitsMin.z, treeSpawnLimitsMax.z)
                        );
                    treeGo.transform.position = pos;
                    break;
                case Creature.Snake:
                    GameObject go = new GameObject("FlockManager");
                    FlockManager fm = go.AddComponent<FlockManager>();
                    fm.boidPrefab = (GameObject)Resources.Load("Prefabs/Boid");
                    fm.boidCardPrefab = (GameObject)Resources.Load("Prefabs/BoidCard");

                    fm.maxSpeed = 5.0f;
                    fm.neighbourDistance = 10.0f;
                    fm.avoidanceFactor = 1.0f;
                    fm.centeringFactor = 0.5f;
                    fm.velocityMatchingFactor = 0.0f;
                    fm.boidTextures = new List<Texture2D> { texture };

                    switch (creatureType)
                    {
                        case Creature.Boid:
                            fm.minSpeed = 0.5f;
                            fm.tailLength = 0;
                            break;
                        case Creature.Snake:
                            fm.minSpeed = 3.0f;
                            fm.tailLength = 20;
                            break;
                    }
                    break;
            }
        }
    }

    private void FileCreatedOrChanged(System.Object sender, FileSystemEventArgs e)
    {
        if(e.FullPath.EndsWith(".png"))
        {
            ProcessPngFile(e.FullPath);
        }
        else if( e.FullPath.EndsWith(".gif"))
        {
            ProcessGifFile(e.FullPath);
        }
    }

    private void ProcessPngFile(String filePath)
    {
        newOrUpdatedFiles.Enqueue(filePath);
    }

    private void ProcessGifFile(String filePath)
    {
        newOrUpdatedGifFiles.Enqueue(filePath);
    }
}
