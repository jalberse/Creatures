using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public static FlockManager FM;

    public GameObject boidPrefab;
    public int numBoids = 20;
    public GameObject[] boids;
    public Vector3 boidLimits = new Vector3(5, 5, 0);
    public Sprite boidSprite;

    [Header ("Boid Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
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
        for(int i = 0; i < numBoids; i++)
        {
            Vector3 pos = this.transform.position + new Vector3
                (
                Random.Range(-boidLimits.x, boidLimits.x),
                Random.Range(-boidLimits.x, boidLimits.x),
                0
                );
            boids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
            boids[i].GetComponent<SpriteRenderer>().sprite = boidSprite;
        }
        FM = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
