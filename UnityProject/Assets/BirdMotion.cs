using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMotion : MonoBehaviour
{
    private float[] velocity;
    private float[] position;
    private float[] force_total;
    private float[] force_to_neighbors;
    private float[] force_to_avoid_neighbors;
    private float[] force_to_align_with_neigbors;
    private float[] distance_buffer;
    private GameObject[] all_birds;

    // Start is called before the first frame update
    void Start()
    {
        UpdateAllBirds();
        velocity = new float[3];
        position = new float[3];
        force_total = new float[3];
        force_to_neighbors = new float[3];
        force_to_avoid_neighbors = new float[3];
        force_to_align_with_neigbors = new float[3];
        distance_buffer = new float[3];
        for (int d = 0; d < 3; d++)
        {
            position[d] = transform.position[d];
        }
        float angle = Mathf.Deg2Rad * transform.eulerAngles[1];
        velocity[0] = -3.0f * Mathf.Sin(angle);
        velocity[1] = 0;
        velocity[2] = -3.0f * Mathf.Cos(angle);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateForces();
        float velocity_magnitude_squared = 0.0f;
        for (int d = 0; d < 3; d++)
        {
            velocity[d] += force_total[d] * Time.deltaTime;
            velocity_magnitude_squared += velocity[d] * velocity[d];
        }
        // don't let birds fly too fast
        if (velocity_magnitude_squared > 30.0f)
        {
            for (int d = 0; d < 3; d++)
            {
                velocity[d] *= 30.0f / velocity_magnitude_squared;
            }
        }
        // don't let birds fly too slow
        if (velocity_magnitude_squared < 10.0f)
        {
            for (int d = 0; d < 3; d++)
            {
                velocity[d] *= 10.0f / velocity_magnitude_squared;
            }
        }
        for (int d = 0; d < 3; d++)
        {
            position[d] += velocity[d] * Time.deltaTime;
        }
        transform.position = new Vector3(position[0], position[1], position[2]);

        float angle_y = 180.0f + Mathf.Rad2Deg * Mathf.Atan2(velocity[0], velocity[2]);
        transform.rotation = Quaternion.Euler(0, angle_y, 0);
    }

    public Vector3 GetVelocityDirection()
    {
        float magnitude_squared = 0.0f;
        for (int d = 0; d < 3; d++)
        {
            magnitude_squared += velocity[d] * velocity[d];
        }
        float velocity_magnitude = Mathf.Sqrt(magnitude_squared);
        return new Vector3(velocity[0] / velocity_magnitude, velocity[1] / velocity_magnitude, velocity[2] / velocity_magnitude);
    }

    private void UpdateForces()
    {
        for (int d = 0; d < 3; d++)
        {
            force_total[d] = 0;
        }
        // birds want to stay close to each other
        UpdateForceToNeighbors();

        // birds want to keep a minimum distance between neighbors
        UpdateForceToAvoidNeighbors();

        // birds want to be oriented similarly to their neighbors
        UpdateForceToAlignWithNeighbors();

        //

        // birds want to stay close to the center of the map
        // TODO

        // birds want to maintain a certain flight height
        // TODO

        for (int d = 0; d < 3; d++)
        {
            force_total[d] += force_to_neighbors[d];
            force_total[d] += force_to_avoid_neighbors[d];
            force_total[d] += force_to_align_with_neigbors[d];
        }

        float magnitude_squared = 0.0f;
        for (int d = 0; d < 3; d++)
        {
            magnitude_squared += force_total[d] * force_total[d];
        }
        // don't let birds accelerate too much
        float threshold = 10.01f;
        if (magnitude_squared > threshold)
        {
            for (int d = 0; d < 3; d++)
            {
                force_total[d] *= threshold / magnitude_squared;
            }
        }
    }

    private void UpdateForceToNeighbors()
    {
        for (int d = 0; d < 3; d++)
        {
            force_to_neighbors[d] = 0;
        }

        for (int i = 0; i < all_birds.Length; i++)
        {
            float distance_squared = 0.0f;
            for (int d = 0; d < 3; d++)
            {
                distance_buffer[d] = transform.position[d] - all_birds[i].transform.position[d];
                distance_squared += distance_buffer[d] * distance_buffer[d];
            }
            if (distance_squared.Equals(0.0f))
            {
                //ourself
                continue;
            }
            for (int d = 0; d < 3; d++)
            {
                force_to_neighbors[d] += -1.0f * distance_buffer[d] / distance_squared;
            }
        }
    }

    public void UpdateForceToAvoidNeighbors()
    {
        for (int d = 0; d < 3; d++)
        {
            force_to_avoid_neighbors[d] = 0;
        }

        for (int i = 0; i < all_birds.Length; i++)
        {
            float distance_squared = 0.0f;
            for (int d = 0; d < 3; d++)
            {
                distance_buffer[d] = transform.position[d] - all_birds[i].transform.position[d];
                distance_squared += distance_buffer[d] * distance_buffer[d];
            }
            if (distance_squared.Equals(0.0f))
            {
                //ourself
                continue;
            }
            if (distance_squared > 15.0f)
            {
                // ignore distant birds
                continue;
            }
            for (int d = 0; d < 3; d++)
            {
                force_to_avoid_neighbors[d] += 100.0f * distance_buffer[d] / distance_squared;
            }
        }
    }

    private void UpdateForceToAlignWithNeighbors()
    {
        for (int d = 0; d < 3; d++)
        {
            force_to_align_with_neigbors[d] = 0;
        }

        for (int i = 0; i < all_birds.Length; i++)
        {
            float distance_squared = 0.0f;
            for (int d = 0; d < 3; d++)
            {
                distance_buffer[d] = transform.position[d] - all_birds[i].transform.position[d];
                distance_squared += distance_buffer[d] * distance_buffer[d];
            }
            if (distance_squared.Equals(0.0f))
            {
                //ourself
                continue;
            }
            if (distance_squared > 30.0f)
            {
                // ignore distant birds and include a little more nearby birds
                continue;
            }
            Vector3 other_bird_velocity_direction = all_birds[i].GetComponent<BirdMotion>().GetVelocityDirection();
            for (int d = 0; d < 3; d++)
            {
                force_to_align_with_neigbors[d] += other_bird_velocity_direction[d]; // include generally nearby birds without taking distance into account
            }
            // normalization
            float magnitude_squared = 0.0f;
            for (int d = 0; d < 3; d++)
            {
                magnitude_squared += force_to_align_with_neigbors[d] * force_to_align_with_neigbors[d];
            }
            float magnitude = Mathf.Sqrt(magnitude_squared);
            for (int d = 0; d < 3; d++)
            {
                force_to_align_with_neigbors[d] = 300.0f * force_to_align_with_neigbors[d] / magnitude;
            }
        }
    }

    public void UpdateAllBirds()
    {
        all_birds = GameObject.FindGameObjectsWithTag("bird");
    }
}
