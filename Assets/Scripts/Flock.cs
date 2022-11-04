using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public float turnSpeed = .01f;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(this.velocity * Time.deltaTime);
        UnityEngine.Quaternion rotationGoal = Quaternion.LookRotation(this.velocity);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, rotationGoal, turnSpeed);
        this.velocity += this.CalculateAcceleration() * Time.deltaTime;
        this.velocity = Utils.ClampMagnitude(this.velocity, FlockManager.FM.maxSpeed, FlockManager.FM.minSpeed);
    }

    Vector3 CalculateAcceleration()
    {
        GameObject[] boids = FlockManager.FM.boids;

        Vector3 acceleration = Vector3.zero;
        foreach(GameObject boid in boids) {
            if(boid == this.gameObject)
            {
                continue;
            }
            float distance = Vector3.Distance(boid.transform.position, this.transform.position);
            if( distance > FlockManager.FM.neighbourDistance)
            {
                continue;
            }

            // The other boid is close enough to influence this boid.
            // Calculate the acceleration on this boid due to the other boid.
            Vector3 u = (boid.transform.position - this.transform.position).normalized;
            // Avoidance
            acceleration += u * (float)(-1.0 * FlockManager.FM.avoidanceFactor / Mathf.Pow(distance, 2.0f));
            // Centering
            acceleration += u * distance * FlockManager.FM.centeringFactor;
            // Velocity Matching 
            Flock otherFlock = boid.GetComponent<Flock>();
            acceleration += FlockManager.FM.velocityMatchingFactor * otherFlock.velocity - this.velocity;
        }

        // Apply acceleration towards the center if we've left the bounds of the simulation.
        Bounds b = new Bounds(FlockManager.FM.transform.position, new Vector3(0, 0, 0));
        b.SetMinMax(FlockManager.FM.boidSpawnLimitsMin, FlockManager.FM.boidSpawnLimitsMax);
        if (!b.Contains(transform.position))
        {
            acceleration += (b.center - this.transform.position);
        }

        return acceleration;
    }
}
