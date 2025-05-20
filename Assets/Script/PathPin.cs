using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathPin : MonoBehaviour
{
    [SerializeField]
    public GameObject[] NearPins;
    GameObject[] guards;
    GameObject[] pathPoints;
    GameObject NextPin;

    private bool inshower;

    private void Start()
    {
        guards = GameObject.FindGameObjectsWithTag("NPC");
        pathPoints = GameObject.FindGameObjectsWithTag("Path");
    }

    private void Update()
    {
        
        NextPin = NearPins[Random.Range(0, NearPins.Length)];
        
        if (this.gameObject.transform == pathPoints[16].transform && inshower)
        {
            NextPin = NearPins[0];
            inshower = false;
        }
        else if (this.gameObject.transform == pathPoints[16].transform && !inshower)
        {
            NextPin = NearPins[Random.Range(1, NearPins.Length)];
            inshower = !inshower;
        }

    }

    public GameObject GetNextPin()
    {
        return this.NextPin;
    }
}
