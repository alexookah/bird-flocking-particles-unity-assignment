using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class ParticleMotionScript : MonoBehaviour {
	float[] particle_states;
	float[] center;
    float[] gravity = { 0, -1.0f, 0 };
    float[] bounds = { -10.0f, 0.0f, -10.0f, 10.0f, 20.0f, 10.0f };
    float damping_percentage = 0.2f;
    public static bool bounds_is_circle = false;

    float[] circle_bounds = { 0.0f, 8.0f, 0.0f, 8.0f };

    [DllImport("ParticlePlugin")]
    protected static extern void physicsStep(float[] particle_states, float[] gravity, float[] bounds, float damping_percentage, int attractors_count,  float[] attractor_data, float dt, bool bounds_is_circle);

	// Use this for initialization
	void Start () {
		particle_states = new float[6];
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

        physicsStep(particle_states, gravity, local_bounds, damping_percentage, attractors_count, attractor_data, dt, bounds_is_circle);
		transform.position = new Vector3 (particle_states[0],particle_states[1],particle_states[2]);

    }
}
