using UnityEngine;
using System.Collections;

public class SpawnerScript : MonoBehaviour {
	public float spawnTime = 3f;		// The amount of time between each spawn.
	public float spawnDelay = 0f;		// The amount of time before spawning starts.
	
	public float max_num_particles = 100;
	
	private GameObject [] object_prefabs;		// Array of prefabs.
	private int number;



    public static int attractors_count = 0;
    public static float[] attractor_data =   { 0, 2.0f, 0, -10.0f, //attractor: 1
                                               0, 8.0f, 8.0f, -10.0f, //attractor: 2
                                               0, 8.0f, -8.0f, -10.0f, //attractor: 3
                                               0, 16.0f, 00, -10.0f, //attractor: 4
                                               };
    private GameObject[] object_attractors_prefabs;       // Array of attractor prefabs.
    private int number_of_attractors_initialized;

    // Use this for initialization
    void Start () {
		object_prefabs = new GameObject[1];
		object_prefabs [0] = Resources.Load<GameObject> ("Prefabs/particle_prefab");
		number = 0;

		InvokeRepeating("Spawn", spawnDelay, spawnTime);


        //show attractors in scene
        object_attractors_prefabs = new GameObject[1];
        object_attractors_prefabs[0] = Resources.Load<GameObject>("Prefabs/attractor_prefab");
        number_of_attractors_initialized = 0;

        InvokeRepeating("AttractorsSpawn", spawnDelay, spawnTime);
    }
	
	void Spawn ()
	{
		if(number >= max_num_particles) return;

		transform.position = new Vector3(0.0f, 0.0f, 0.0f);

		float r = 0.0f;
        float spawnerHeight = 10.0f;
		float x_perturb = Random.Range (-r, r);
		float y_perturb = Random.Range (-r, r);
		float z_perturb = Random.Range (-r, r);
		Vector3 pos = new Vector3(transform.position.x+x_perturb, transform.position.y+y_perturb + spawnerHeight, transform.position.z+z_perturb);
        //Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Instantiate(object_prefabs[0], pos, transform.rotation);
		number++;
    }

    void AttractorsSpawn()
    {
        if (attractors_count == number_of_attractors_initialized) return;

        //remove all first
        GameObject[] goDestroy = GameObject.FindGameObjectsWithTag("attractor");
        foreach (GameObject goHolder in goDestroy) Destroy(goHolder);
        number_of_attractors_initialized = 0;

        for (int i = 0; i < attractors_count; i++)
        {
            float x_perturb = attractor_data[4 * i];
            float y_perturb = attractor_data[4 * i + 1];
            float z_perturb = attractor_data[4 * i + 2];

            Vector3 pos = new Vector3(x_perturb, y_perturb, z_perturb);

            Instantiate(object_attractors_prefabs[0], pos, transform.rotation);
            number_of_attractors_initialized += 1;
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}