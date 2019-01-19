using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class ParticleMotionScript : MonoBehaviour {
	float[] particle_states;

    float[] all_particles_data;
    int particles_count;

    float[] center;
    float[] gravity = { 0, -2.0f, 0 };
    public static float[] bounds = { -10.0f, 0.0f, -10.0f, 10.0f, 20.0f, 10.0f };
    public static float damping_percentage = 0.2f;
    public static bool bounds_is_circle = false;

    public static float[] circle_bounds = { 0.0f, 8.0f, 0.0f, 8.0f };

    [DllImport("ParticlePlugin")]
    protected static extern void physicsStep(float[] particle_states, float[] all_particles_data, int particles_count, float[] gravity, float[] bounds, float damping_percentage, int attractors_count,  float[] attractor_data, float dt, bool bounds_is_circle);

	// Use this for initialization
	void Start () {
		particle_states = new float[6];

        //GameObject[] all_particles = GameObject.FindGameObjectsWithTag("particle");
        all_particles_data = new float[4 * 99];

        center = new float[3];

		////position
		for(int d=0;d<3;d++) particle_states[d]=transform.position[d];

		////velocity
		particle_states[0 + 3] = UnityEngine.Random.Range(-3.0f, 3.0f);
        particle_states[1 + 3] = UnityEngine.Random.Range(-3.0f, 3.0f);
        particle_states[2 + 3] = UnityEngine.Random.Range(-3.0f, 3.0f);

        ////center
        center[0] = 0;
        center[1] = 0;
        center[2] = particle_states[2];
    }
	
	void FixedUpdate () {
		float dt = Time.deltaTime;

        int attractors_count = SpawnerScript.attractors_count;
        float[] attractor_data = SpawnerScript.attractor_data;

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


        physicsStep(particle_states, all_particles_data, particles_count, gravity, local_bounds, damping_percentage, attractors_count, attractor_data, dt, bounds_is_circle);

		transform.position = new Vector3 (particle_states[0],particle_states[1],particle_states[2]);

    }
}
