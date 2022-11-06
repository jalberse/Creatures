using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public GameObject boidCardPrefab;
    public int numBoids = 4;
    public GameObject[] boids;
    // The boidCards provide the additional sprites that are children of the boid,
    //   which create the 3D effect of the sprites.
    public GameObject[] boidCards;
    public Vector3 boidSpawnLimitsMin = new Vector3(-7, 2, -2);
    public Vector3 boidSpawnLimitsMax = new Vector3(7, 12, 18);
    public Texture2D boidTexture;

    public uint tailLength;

    [Header ("Boid Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(1.0f, 25.0f)]
    public float neighbourDistance;
    [Range(0.0f, 1.0f)]
    public float avoidanceFactor;
    [Range(0.0f, 1.0f)]
    public float centeringFactor;
    [Range(0.0f, 1.0f)]
    public float velocityMatchingFactor;
    

    // Start is called before the first frame update
    void Start()
    {
        boids = new GameObject[numBoids];
        boidCards = new GameObject[numBoids * 3];
        for(int i = 0; i < numBoids; i++)
        {
            Vector3 pos = this.transform.position + new Vector3
                (
                Random.Range(boidSpawnLimitsMin.x, boidSpawnLimitsMax.x),
                Random.Range(boidSpawnLimitsMin.y, boidSpawnLimitsMax.y),
                Random.Range(boidSpawnLimitsMin.z, boidSpawnLimitsMax.z)
                );
            boids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
            boids[i].GetComponent<MeshRenderer>().material.mainTexture = boidTexture;
            boids[i].GetComponent<Flock>().FM = this;
            for (int j = 0; j < tailLength; j++)
            {
                GameObject tailNode = Instantiate(boidCardPrefab, boids[i].transform.position, Quaternion.identity);
                tailNode.GetComponent<MeshRenderer>().material.mainTexture = boidTexture;
                boids[i].GetComponent<Flock>().tail.Add(tailNode);
            }

            boidCards[i] = Instantiate(boidCardPrefab, pos, Quaternion.identity);
            Vector3 position = boidCards[i].GetComponent<Renderer>().bounds.center;
            boidCards[i].transform.RotateAround(position, Vector3.up, 90);
            boidCards[i].transform.parent = boids[i].transform;
            boidCards[i].GetComponent<MeshRenderer>().material.mainTexture = boidTexture;

            boidCards[i + numBoids] = Instantiate(boidCardPrefab, pos, Quaternion.identity);
            position = boidCards[i + numBoids].GetComponent<Renderer>().bounds.center;
            boidCards[i + numBoids].transform.RotateAround(position, Vector3.up, 180);
            boidCards[i + numBoids].transform.parent = boids[i].transform;
            boidCards[i + numBoids].GetComponent<MeshRenderer>().material.mainTexture = boidTexture;

            boidCards[i + numBoids * 2] = Instantiate(boidCardPrefab, pos, Quaternion.identity);
            position = boidCards[i + numBoids * 2].GetComponent<Renderer>().bounds.center;
            boidCards[i + numBoids * 2].transform.RotateAround(position, Vector3.up, 270);
            boidCards[i + numBoids * 2].transform.parent = boids[i].transform;
            boidCards[i + numBoids * 2].GetComponent<MeshRenderer>().material.mainTexture = boidTexture;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
