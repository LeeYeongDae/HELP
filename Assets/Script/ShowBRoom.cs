using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShowBRoom : MonoBehaviour
{

    PlayableChr player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find("Player").GetComponent<PlayableChr>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (this.gameObject.tag == "BackRoom")
                this.gameObject.GetComponent<TilemapRenderer>().enabled = false;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (this.gameObject.tag == "BackRoom")
            {
                this.gameObject.GetComponent<TilemapRenderer>().enabled = true;
            }
        }
    }
}
