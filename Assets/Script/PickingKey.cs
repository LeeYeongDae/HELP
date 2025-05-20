using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingKey : MonoBehaviour
{
    MiniGame minigame;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        minigame = GameObject.Find("MiniGame").GetComponent<MiniGame>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Detect");
            minigame.isFail = true;
        }
    }
}
