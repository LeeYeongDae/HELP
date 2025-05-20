using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingObstacle : MonoBehaviour
{
    public int speed;
    public bool upperMove;
    Vector3 dir;
    void Start()
    {
        dir = Vector3.down;
        if (upperMove) dir = Vector3.up;
        speed = new System.Random().Next(1, 5);
    }

    void Update()
    {
        float walk = speed * Time.deltaTime;
        transform.position += dir * speed;
        if(upperMove)
        {
            if (this.gameObject.transform.localPosition.y >= 75)
                dir = Vector3.down;
            else if (this.gameObject.transform.localPosition.y <= 0)
                dir = Vector3.up;
        }
        else
        {
            if (this.gameObject.transform.localPosition.y <= -75)
                dir = Vector3.up;
            else if (this.gameObject.transform.localPosition.y >= 0)
                dir = Vector3.down;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PassKey"))
        {
            speed = 0;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PassKey"))
        {
            speed = new System.Random().Next(1, 5);
        }
    }
}
