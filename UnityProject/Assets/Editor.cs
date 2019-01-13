using UnityEngine;
using UnityEditor;

public class MenuItems
{
    [MenuItem("Tools/1 Gravity Behaviour")]
    private static void NewMenuOption0()
    {
        SpawnerScript.attractors_count = 0;
    }

    // Behaviour 2
    [MenuItem("Tools/2 Attractors Behaviour/size: 1")]
    private static void NewMenuOption1()
    {
        SpawnerScript.attractors_count = 1;

        GameObject spawner = GameObject.FindWithTag("spawner");
    }

    [MenuItem("Tools/2 Attractors Behaviour/size: 2")]
    private static void NewMenuOption2()
    {
        SpawnerScript.attractors_count = 2;
    }

    [MenuItem("Tools/2 Attractors Behaviour/size: 3")]
    private static void NewMenuOption3()
    {
        SpawnerScript.attractors_count = 3;
    }

    [MenuItem("Tools/2 Attractors Behaviour/size: 4")]
    private static void NewMenuOption4()
    {
        SpawnerScript.attractors_count = 4;

    }

    // Behaviour 3
    [MenuItem("Tools/3 Toggle Bounds is Circle")]
    private static void NewMenuOption5()
    {
        if (ParticleMotionScript.bounds_is_circle)
        {
            ParticleMotionScript.bounds_is_circle = false;
            GameObject cube = GameObject.FindWithTag("bounds_cube");
            cube.SetActive(false);

            GameObject sphere = GameObject.FindWithTag("bounds_sphere");
            sphere.SetActive(true);

        }
        else
        {
            ParticleMotionScript.bounds_is_circle = true;

            GameObject cube = GameObject.FindWithTag("bounds_cube");
            cube.SetActive(true);

            GameObject sphere = GameObject.FindWithTag("bounds_sphere");
            sphere.SetActive(false);
        }
    }

    [MenuItem("Tools/Spawn Bird")]
    private static void NewMenuOption6()
    {
        GameObject spawner = GameObject.FindWithTag("spawner");
        spawner.GetComponent<SpawnerScript>().SpawnBirds();
    }
}