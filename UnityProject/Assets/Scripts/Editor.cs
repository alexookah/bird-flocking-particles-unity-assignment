using UnityEngine;
using UnityEditor;

public class MenuItems
{
    [MenuItem("Tools/Attractors Behaviour/Gravity only - size: 0")]
    private static void NewMenuOption0()
    {
        SpawnerScript.attractors_count = 0;
    }

    // Behaviour 2
    [MenuItem("Tools/Attractors Behaviour/size: 1")]
    private static void NewMenuOption1()
    {
        SpawnerScript.attractors_count = 1;

        GameObject spawner = GameObject.FindWithTag("spawner");

        CheckBehaviour();
    }

    [MenuItem("Tools/Attractors Behaviour/size: 2")]
    private static void NewMenuOption2()
    {
        SpawnerScript.attractors_count = 2;

        CheckBehaviour();
    }

    [MenuItem("Tools/Attractors Behaviour/size: 3")]
    private static void NewMenuOption3()
    {
        SpawnerScript.attractors_count = 3;

        CheckBehaviour();
    }

    [MenuItem("Tools/Attractors Behaviour/size: 4")]
    private static void NewMenuOption4()
    {
        SpawnerScript.attractors_count = 4;

        CheckBehaviour();
    }

    // Behaviour 3
    [MenuItem("Tools/Toggle Bounds is Circle")]
    private static void NewMenuOption5()
    {
        ToggleCircle();
        SpawnerScript.attractors_count = 0;
    }

    [MenuItem("Tools/Spawn Birds")]
    private static void NewMenuOption6()
    {
        GameObject spawner = GameObject.FindWithTag("spawner");
        spawner.GetComponent<SpawnerScript>().SpawnBirds();
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
}