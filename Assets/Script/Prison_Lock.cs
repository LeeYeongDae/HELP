using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison_Lock : MonoBehaviour
{
    public GameObject door;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (InGameManager.nightguard)
            door.gameObject.SetActive(true);
        else
            door.gameObject.SetActive(false);
    }
}
