using UnityEngine;

public class ButtonsActions : MonoBehaviour
{

    GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Attractors_size_0()
    {
        SpawnerScript.attractors_count = 0;;

        ShowAttractorMessage();
    }

    // Behaviour 2
    public void Attractors_size_1()
    {
        SpawnerScript.attractors_count = 1;

        GameObject spawner = GameObject.FindWithTag("spawner");

        CheckBehaviour();

        ShowAttractorMessage();
    }

    public void Attractors_size_2()
    {
        SpawnerScript.attractors_count = 2;

        CheckBehaviour();

        ShowAttractorMessage();
    }

    public void Attractors_size_3()
    {
        SpawnerScript.attractors_count = 3;

        CheckBehaviour();

        ShowAttractorMessage();
    }

    public void Attractors_size_4()
    {
        SpawnerScript.attractors_count = 4;

        CheckBehaviour();

        ShowAttractorMessage();

    }

    private void ShowAttractorMessage()
    {
        string msg = "Attractors size is: " + SpawnerScript.attractors_count;

        camera.GetComponent<CameraMove>().SetMessage(msg);
        camera.GetComponent<CameraMove>().ShowMessage();
    }

    public void ToggleRepulsion()
    {
        ParticleMotionScript.repulsionIsEnabled = !ParticleMotionScript.repulsionIsEnabled;


        string msg = ParticleMotionScript.repulsionIsEnabled ? "Repulsion is enabled" : "Repulsion is disabled";
        	
        camera.GetComponent<CameraMove>().SetMessage(msg);
        camera.GetComponent<CameraMove>().ShowMessage();
    }

    // Behaviour 3
    public void ToggleCircleOrCube()
    {
        ToggleCircle();
        SpawnerScript.attractors_count = 0;

        string msg = ParticleMotionScript.bounds_is_circle ? "Circle is enabled" : "Cube is enabled";

        camera.GetComponent<CameraMove>().SetMessage(msg);
        camera.GetComponent<CameraMove>().ShowMessage();
    }

    public void SpawnBirds()
    {
        GameObject spawner = GameObject.FindWithTag("spawner");
        spawner.GetComponent<SpawnerScript>().SpawnBirds();

        string msg = "birds were created...";

        camera.GetComponent<CameraMove>().SetMessage(msg);
        camera.GetComponent<CameraMove>().ShowMessage();
    }


    private static void CheckBehaviour()
    {
        //toggle into cube for behaviour 1
        if (ParticleMotionScript.bounds_is_circle)
        {
            ToggleCircle();
        }
    }

    private static void ToggleCircle()
    {
        if (ParticleMotionScript.bounds_is_circle)
        {
            Debug.Log("Bounds is circle == true");
            ParticleMotionScript.bounds_is_circle = false;

            GameObject spawner = GameObject.FindWithTag("spawner");

            GameObject cube = spawner.GetComponent<SpawnerScript>().cube;
            cube.SetActive(true);

            GameObject sphere = spawner.GetComponent<SpawnerScript>().sphere;
            sphere.SetActive(false);

        }
        else
        {
            ParticleMotionScript.bounds_is_circle = true;

            GameObject spawner = GameObject.FindWithTag("spawner");

            GameObject cube = spawner.GetComponent<SpawnerScript>().cube;
            cube.SetActive(false);

            GameObject sphere = spawner.GetComponent<SpawnerScript>().sphere;
            sphere.SetActive(true);
        }
    }


    public void SpawnBalls()
    {
        GameObject spawner = GameObject.FindWithTag("spawner");
        spawner.GetComponent<SpawnerScript>().max_num_particles += 5;

        string msg = "You got balls...";

        camera.GetComponent<CameraMove>().SetMessage(msg);
        camera.GetComponent<CameraMove>().ShowMessage();
    }
}
