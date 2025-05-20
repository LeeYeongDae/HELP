using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison_Lock : MonoBehaviour
{
    public GameObject door;
    InGameManager inGame;
    public bool lock_state;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        inGame = GameObject.Find("GameManager").GetComponent<InGameManager>();
        lock_state = inGame.nightguard;
        if (lock_state == true)
            door.gameObject.SetActive(true);
        else
            door.gameObject.SetActive(false);
    }
}
