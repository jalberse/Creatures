using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager FM;

    public GameObject boidPrefab;
    public GameObject boidCardPrefab;
    public int numBoids = 4;
    public GameObject[] boids;
    // The boidCards provide the additional sprites that are children of the boid,
    //   which create the 3D effect of the sprites.
    public GameObject[] boidCards;
    public Vector3 boidSpawnLimitsMin = new Vector3(-7, 2, -2);
    public Vector3 boidSpawnLimitsMax = new Vector3(7, 12, 18);
    public Sprite boidSprite;

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
            boids[i].GetComponent<SpriteRenderer>().sprite = boidSprite;

            boidCards[i] = Instantiate(boidCardPrefab, pos, Quaternion.LookRotation(Vector3.left + Vector3.up));
            boidCards[i].transform.parent = boids[i].transform;
            boidCards[i].GetComponent<SpriteRenderer>().sprite = boidSprite;

            boidCards[i + numBoids] = Instantiate(boidCardPrefab, pos, Quaternion.LookRotation(Vector3.right + Vector3.up));
            boidCards[i + numBoids].transform.parent = boids[i].transform;
            boidCards[i + numBoids].GetComponent<SpriteRenderer>().sprite = boidSprite;

            boidCards[i + numBoids * 2] = Instantiate(boidCardPrefab, pos, Quaternion.LookRotation(Vector3.back + Vector3.up));
            boidCards[i + numBoids * 2].transform.parent = boids[i].transform;
            boidCards[i + numBoids * 2].GetComponent<SpriteRenderer>().sprite = boidSprite;
        }
        FM = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
