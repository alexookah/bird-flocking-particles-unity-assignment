using UnityEngine;
using System.Runtime.InteropServices;

public class ParticleMotionScript : MonoBehaviour
{
    float[] particle_states;

    float[] all_particles_data;
    int particles_count;

    float[] center;
    float[] gravity = { 0, -2.0f, 0 };
    public static float[] bounds = { -10.0f, 0.0f, -10.0f, 10.0f, 20.0f, 10.0f };
    public static float damping_percentage = 0.2f;
    public static bool bounds_is_circle = false;
    public static bool repulsionIsEnabled = false;

    public static float[] circle_bounds = { 0.0f, 8.0f, 0.0f, 8.0f };


    [DllImport("ParticlePlugin")]
    protected static extern void physicsStep(float[] particle_states, float[] all_particles_data, int particles_count, float[] gravity, float[] bounds, float damping_percentage, int attractors_count, float[] attractor_data, float dt, bool bounds_is_circle);

    // Use this for initialization
    void Start()
    {


        particle_states = new float[6];

        //GameObject[] all_particles = GameObject.FindGameObjectsWithTag("particle");
        all_particles_data = new float[4 * 99];

        center = new float[3];

        ////position
        for (int d = 0; d < 3; d++) particle_states[d] = transform.position[d];

        ////velocity
        particle_states[0 + 3] = UnityEngine.Random.Range(-3.0f, 3.0f);
        particle_states[1 + 3] = UnityEngine.Random.Range(-3.0f, 3.0f);
        particle_states[2 + 3] = UnityEngine.Random.Range(-3.0f, 3.0f);

        ////center
        center[0] = 0;
        center[1] = 0;
        center[2] = particle_states[2];
    }

    void FixedUpdate()
    {

        float[] local_bounds = bounds;
        if (bounds_is_circle)
        {
            local_bounds = circle_bounds;
        }

        GameObject[] all_particles = GameObject.FindGameObjectsWithTag("particle");

        particles_count = all_particles.Length;

        ////position
        for (int i = 0; i < particles_count; i++)
        {
            for (int d = 0; d < 3; d++)
            {
                all_particles_data[4 * i + d] = all_particles[i].transform.position[d];
            }
            all_particles_data[4 * i + 3] = -10.0f;
        }

        if (!repulsionIsEnabled)
        {
            //to disable repulsion in physics Step
            particles_count = 0;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        physicsStepForWebGL();
#else
        int attractors_count = SpawnerScript.attractors_count;
        float[] attractor_data = SpawnerScript.attractor_data;
        float dt = Time.deltaTime;

        physicsStep(particle_states, all_particles_data, particles_count, gravity, local_bounds, damping_percentage, attractors_count, attractor_data, dt, bounds_is_circle);

#endif


        transform.position = new Vector3(particle_states[0], particle_states[1], particle_states[2]);

    }




    void physicsStepForWebGL()
    {
        float[] x = { particle_states[0], particle_states[1], particle_states[2] };
        float[] v = { particle_states[3], particle_states[4], particle_states[5] };
        float[] a = new float[3];

        int attractors_count = SpawnerScript.attractors_count;
        float[] attractor_data = SpawnerScript.attractor_data;
        float dt = Time.deltaTime;

        //attractos will have:

        //float gravity[3] = {0, -1.0f, 0};
        //float bounds[6] = {-10.0f, 0.0f, -10.0f, 10.0f, 20.0f, 10.0f};

        // /* calculating forces */
        // attractions from attractors
        for (int i = 0; i < attractors_count; i++)
        {
            float distance_squared = 0.0f;
            float[] distance = new float[3];
            for (int d = 0; d < 3; d++)
            {
                distance[d] = x[d] - attractor_data[4 * i + d];
                distance_squared += distance[d] * distance[d];
            }
            //distance_cubed *= std::sqrt(distance_cubed);
            for (int d = 0; d < 3; d++)
            {
                a[d] += attractor_data[4 * i + 3] * distance[d] / distance_squared;
            }
        }

        // repulsions between particles
        for (int i = 0; i < particles_count; i++)
        {
            float distance_squared = 0.0f;
            float[] distance = new float[3];
            for (int d = 0; d < 3; d++)
            {
                distance[d] = x[d] - all_particles_data[4 * i + d];
                distance_squared += distance[d] * distance[d];
            }
            if (distance_squared < 0.001f)
            {
                continue;
            }
            for (int d = 0; d < 3; d++)
            {
                a[d] -= all_particles_data[4 * i + 3] * distance[d] / distance_squared;
            }
        }

        // gravity
        for (int d = 0; d < 3; d++)
        {
            a[d] += gravity[d];
        }
        // /* DONE calculating forces */

        for (int d = 0; d < 3; d++)
        {
            v[d] += a[d] * dt;
            x[d] += v[d] * dt;
        }

        if (bounds_is_circle)
        {

            float circle_x = circle_bounds[0];
            float circle_y = circle_bounds[1];
            float circle_z = circle_bounds[2];
            float circle_r = circle_bounds[3];

            float dist_X = x[0] - circle_x;
            float dist_Y = x[1] - circle_y;
            float dist_Z = x[2] - circle_z;

            float r_2_after = dist_X * dist_X + dist_Y * dist_Y + dist_Z * dist_Z;

            if (r_2_after > circle_r * circle_r)
            {
                float force_magnitude = (2 - damping_percentage) * (v[0] * dist_X + v[1] * dist_Y + v[2] * dist_Z) / (circle_r * circle_r);

                v[0] -= force_magnitude * dist_X;
                v[1] -= force_magnitude * dist_Y;
                v[2] -= force_magnitude * dist_Z;

                float ratio = circle_r / Mathf.Sqrt(r_2_after);
                x[0] = circle_x + dist_X * ratio;
                x[1] = circle_y + dist_Y * ratio;
                x[2] = circle_z + dist_Z * ratio;
            }

        }
        else
        {

            float[] bounds_low = { bounds[0], bounds[1], bounds[2] };
            float[] bounds_high = { bounds[3], bounds[4], bounds[5] };

            for (int d = 0; d < 3; d++)
            {
                if (x[d] < bounds_low[d])
                {
                    v[d] = -v[d] * (1.0f - damping_percentage);
                    x[d] += 2 * (bounds_low[d] - x[d]);
                }
                if (x[d] > bounds_high[d])
                {
                    v[d] = -v[d] * (1.0f - damping_percentage);
                    x[d] += 2 * (bounds_high[d] - x[d]);
                }
            }
        }

        particle_states[0] = x[0];
        particle_states[1] = x[1];
        particle_states[2] = x[2];

        particle_states[3] = v[0];
        particle_states[4] = v[1];
        particle_states[5] = v[2];
    }
}