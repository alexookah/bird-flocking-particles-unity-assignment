using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMotion : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 position;
    private Vector3 force_total;
    private Vector3 force_to_align_with_neigbors;
    private Vector3 force_to_avoid_collision;
    private Vector3 force_for_cohesion;
    private Vector3 force_of_leadership;
    private Vector3 force_of_air_drag;
    private Vector3 force_stay_in_the_map;
    private float[] all_distances;

    private static float SCARY_SMALL_DISTANCE = 4.0f;
    private static float MAX_ACCELERATION = 10.0f;
    private static float ALIGNMENT_VISIBILITY_RADIUS = 16.0f;
    private static float SCARY_MISALIGNMENT_ANGLE_IN_DEG = 35.0f;
    private static float COHESION_VISIBILITY_RADIUS = 100.0f;
    private static float MIN_VELOCITY_TO_CARE_ABOUT_ALIGNMENT = 2.0f;
    private static float AIR_DRAG_COEFFICIENT = 0.05f;
    private static float STAY_IN_THE_MAP_RADIUS_START = 180.0f;
    private static float STAY_IN_THE_MAP_SCALE = 0.1f;

    private GameObject closest_bird;


    private static float LEADERSHIP_SCAN_RADIUS = 30.0f;

    private Vector3[] axial_distances_from_other_birds;
    private GameObject[] other_birds;

    Animator m_Animator;

    private bool animationIsFlyStay = true; //starting with animation: FlyStay

    // Start is called before the first frame update
    void Start()
    {
        UpdateAllBirds();

        velocity = new Vector3();
        position = new Vector3();
        force_total = new Vector3();
        force_to_align_with_neigbors = new Vector3();
        force_for_cohesion = new Vector3();
        force_of_leadership = new Vector3();
        force_of_air_drag = new Vector3();
        force_stay_in_the_map = new Vector3();
        for (int d = 0; d < 3; d++)
        {
            position[d] = transform.position[d];
        }
        float angle = Mathf.Deg2Rad * transform.eulerAngles[1];
        velocity[0] = -3.0f * Mathf.Sin(angle);
        velocity[1] = 0;
        velocity[2] = -3.0f * Mathf.Cos(angle);

        m_Animator = gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistances();
        UpdateForces();
        UpdateVelocity();
        UpdatePosition();
        UpdateTransform();


        UpdateAnimation();



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
            force_to_avoid_collision[d] = 0;
            force_to_align_with_neigbors[d] = 0;
            force_for_cohesion[d] = 0;
            force_of_leadership[d] = 0;
            force_of_air_drag[d] = 0;
            force_stay_in_the_map[d] = 0;
        }

        //if my closest bird is too close, avoid it
        if (AvoidCollisions())
        {
            return;
        }

        //if I am too misaligned, align myself
        if (AlignMyself())
        {
            return;
        }
        if (IAmLeader())
        {
            UpdateLeadershipForce();
        }
        else
        {
            UpdateCohesionForce();
        }

        // birds want to stay close to the center of the map
        // TODO

        // birds want to maintain a certain flight height
        // TODO

        // Air drag
        UpdateAirDragForce();
        UpdateForceStayInTheMap();
    }

    private void UpdateAirDragForce()
    {
        force_of_air_drag = -velocity * velocity.magnitude * AIR_DRAG_COEFFICIENT;
    }

    private void UpdateForceStayInTheMap()
    {
        if (position.magnitude < STAY_IN_THE_MAP_RADIUS_START)
        {
            return;
        }
        else
        {
            float distanceFromMinRadius = position.magnitude - STAY_IN_THE_MAP_RADIUS_START;
            float force_magnitude = distanceFromMinRadius * STAY_IN_THE_MAP_SCALE;
            Vector3 forceDirection = Vector3.Cross(position, new Vector3(0, 1, 0)).normalized;
            force_stay_in_the_map = forceDirection * force_magnitude;
        }


    }

    private bool IAmLeader()
    {
        // find birds in LEADERSHIP_SCAN_RADIUS radius
        int nearby_birds_count = 0;
        int[] nearby_bird_ids = new int[other_birds.Length];
        for (int i = 0; i < other_birds.Length; i++)
        {
            if (all_distances[i] < LEADERSHIP_SCAN_RADIUS)
            {
                nearby_bird_ids[nearby_birds_count] = i;
                nearby_birds_count++;
            }
        }
        // too few birds to be a leader
        if (nearby_birds_count < 10)
        {
            return false;
        }

        // Check if nearby birds are behind me
        for (int i = 0; i < nearby_birds_count; i++)
        {
            Vector3 other_bird_position = other_birds[nearby_bird_ids[i]].GetComponent<BirdMotion>().position;

            bool is_behind_me = Vector3.Dot(velocity, other_bird_position - position) > 0;
            if (!is_behind_me)
            {
                return false;
            }
        }

        // Check if nearby birds are moving towards me
        Vector3 other_birds_total_velocity = new Vector3();
        for (int i = 0; i < nearby_birds_count; i++)
        {
            other_birds_total_velocity += other_birds[nearby_bird_ids[i]].GetComponent<BirdMotion>().velocity;
        }
        Vector3 other_birds_average_velocity = other_birds_total_velocity / nearby_birds_count;
        float leadership_pushing_threshold = 0.0f;
        bool other_birds_are_pushing = Vector3.Dot(velocity, other_birds_average_velocity) > leadership_pushing_threshold;
        if (!other_birds_are_pushing)
        {
            return false;
        }

        return true;
    }


    private void UpdateLeadershipForce()
    {
        force_of_leadership = velocity.normalized * MAX_ACCELERATION * 0.5f;
    }


    private void UpdateVelocity()
    {
        force_total += force_to_avoid_collision;
        force_total += force_to_align_with_neigbors;
        force_total += force_for_cohesion;
        force_total += force_of_leadership;
        force_total += force_of_air_drag;
        force_total += force_stay_in_the_map;

        Vector3 delta_v = force_total * Time.deltaTime;
        velocity += delta_v;         
    }


    private void UpdatePosition()
    {
        position += velocity * Time.deltaTime;
    }


    private void UpdateAnimation()
    {
        bool animationShouldBeFly = force_total.magnitude > 1; //0.001f * MAX_ACCELERATION;

        if (animationIsFlyStay && animationShouldBeFly)
        {
            //Debug.Log("FLY");
            m_Animator.SetTrigger("Fly");
            animationIsFlyStay = false;

        } else if (!animationIsFlyStay && !animationShouldBeFly)
        {
            //Debug.Log("FLYSTAY");
            m_Animator.SetTrigger("FlyStay");
            animationIsFlyStay = true;
        }

        //Update Speed
        m_Animator.speed = (velocity.magnitude * 0.2f) + 0.9f;

    }


    private void UpdateTransform()
    {
        transform.position = position;
        float angle_y =  Mathf.Rad2Deg * Mathf.Atan2(velocity[0], velocity[2]);
        transform.rotation = Quaternion.Euler(0, angle_y, 0);
    }


    private bool AvoidCollisions()
    {
        if (other_birds.Length == 0)
        {
            return false;
        }
        int closest_bird_index = 0;
        float minimum_distance = -1;
        for (int i = 0; i < other_birds.Length; i++)
        {
            if (minimum_distance < 0 || all_distances[i] < minimum_distance)
            {
                minimum_distance = all_distances[i];
                closest_bird_index = i;
            }
        }
        Vector3 position_to_avoid = other_birds[closest_bird_index].transform.position;
        if (transform.position.y < minimum_distance)
        {
            // the closest obstacle is the terrain
            position_to_avoid = new Vector3(transform.position.x, 0, transform.position.z);
            minimum_distance = transform.position.y;
        }

        if (minimum_distance < SCARY_SMALL_DISTANCE)
        {
            // avoid the obstacle at all costs
            Vector3 vector_distance = transform.position - other_birds[closest_bird_index].transform.position;
            force_to_avoid_collision = MAX_ACCELERATION * vector_distance / minimum_distance;
            return true;
        }

        return false;
    }


    private bool AlignMyself()
    {
        // find all birds within a certain radius
        if (other_birds.Length == 0 || Vector3.Magnitude(velocity) < MIN_VELOCITY_TO_CARE_ABOUT_ALIGNMENT)
        {
            return false;
        }
        int nearby_birds_count = 0;
        int[] nearby_bird_ids = new int[other_birds.Length];
        for (int i = 0; i < other_birds.Length; i++)
        {
            if (all_distances[i] < ALIGNMENT_VISIBILITY_RADIUS)
            {
                nearby_bird_ids[nearby_birds_count] = i;
                nearby_birds_count++;
            }
        }
        // calculate the average velocity of these birds
        Vector3 total_velocity = new Vector3(0, 0, 0);
        for (int i = 0; i < nearby_birds_count; i++)
        {
            total_velocity += other_birds[i].GetComponent<BirdMotion>().velocity;
        }
        Vector3 normalized_total_velocity = Vector3.Normalize(total_velocity);
        // calculate direction difference between this velocity and my velocity
        float angle = Vector3.Angle(velocity, normalized_total_velocity);
        // if the direction difference is too big, make a force to align myself
        if (angle > SCARY_MISALIGNMENT_ANGLE_IN_DEG)
        {
            Vector3 velocity_difference = normalized_total_velocity - velocity;
            force_to_align_with_neigbors = MAX_ACCELERATION * Vector3.Normalize(velocity_difference);
            return true;
        }

        force_to_align_with_neigbors = normalized_total_velocity * MAX_ACCELERATION * 0.6f;

        return false;
    }


    private void UpdateCohesionForce()
    {
        // find all birds within a certain radius
        if (other_birds.Length == 0)
        {
            return;
        }
        int nearby_birds_count = 0;
        int[] nearby_bird_ids = new int[other_birds.Length];
        for (int i = 0; i < other_birds.Length; i++)
        {
            if (all_distances[i] < COHESION_VISIBILITY_RADIUS)
            {
                nearby_bird_ids[nearby_birds_count] = i;
                nearby_birds_count++;
            }
        }
        // calculate the average position of these birds
        Vector3 total_position = new Vector3(0, 0, 0);
        for (int i = 0; i < nearby_birds_count; i++)
        {
            total_position += other_birds[nearby_bird_ids[i]].GetComponent<BirdMotion>().position;
        }
        Vector3 average_position = total_position / nearby_birds_count;
        // find local birds (between me and the average position)
        Vector3 local_sphere_center = 0.5f * (position + average_position);
        float local_sphere_radius = 0.5f * Vector3.Magnitude(position - average_position);
        int local_birds_count = 0;
        int[] local_bird_ids = new int[other_birds.Length];
        for (int i = 0; i < other_birds.Length; i++)
        {
            if (Vector3.Distance(local_sphere_center, other_birds[i].transform.position) < local_sphere_radius)
            {
                local_bird_ids[local_birds_count] = i;
                local_birds_count++;
            }
        }
        // find the average position of the local birds only
        Vector3 local_total_position = new Vector3(0, 0, 0);
        for (int i = 0; i < local_birds_count; i++)
        {
            local_total_position += other_birds[local_bird_ids[i]].GetComponent<BirdMotion>().position;
        }
        Vector3 local_average_position = local_total_position / local_birds_count;
        // if local birds position is close to me, ok
        float distance_from_local_center = Vector3.Distance(local_average_position, local_sphere_center);
        float distance_from_global_center = Vector3.Distance(local_average_position, average_position);
        if (distance_from_local_center < 0.5f * distance_from_global_center)
        {
            // all good, there are many birds between me and the global center
            Vector3 pd = average_position - position;
            force_for_cohesion = 0.1f * MAX_ACCELERATION * Vector3.Normalize(pd);
            return;
        }
        // make a force towards this position
        Vector3 position_difference = average_position - position;
        force_for_cohesion = MAX_ACCELERATION * Vector3.Normalize(position_difference);
    }

    // This function lets THIS bird know which are the other birds
    public void UpdateAllBirds()
    {
        GameObject[] all_birds = GameObject.FindGameObjectsWithTag("bird");
        other_birds = new GameObject[all_birds.Length - 1];
        int birds_count = 0;
        for (int i = 0; i < all_birds.Length; i++)
        {
            if (Vector3.Distance(transform.position, all_birds[i].transform.position) > 0)
            {
                other_birds[birds_count] = all_birds[i];
                birds_count++;
            }
        }
        all_distances = new float[birds_count];
        axial_distances_from_other_birds = new Vector3[birds_count];
    }

    // This function calculates distance from all other birds (and separately for each axis: x, y, z)
    void CalculateDistances()
    {
        for (int i = 0; i < other_birds.Length; i++)
        {
            float distance = Vector3.Distance(other_birds[i].transform.position, transform.position);
            all_distances[i] = distance;

            Vector3 dist_axis = other_birds[i].transform.position - transform.position;
            axial_distances_from_other_birds[i] = dist_axis;
        }
    }
}
