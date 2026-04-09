using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodes : MonoBehaviour
{
    int numToSpawn = 25;
    public float spawnOffset = 0.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.name == "Node")
        {
            for (int i = 0; i < numToSpawn; i++)
            {
                GameObject clone = Instantiate(gameObject, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
