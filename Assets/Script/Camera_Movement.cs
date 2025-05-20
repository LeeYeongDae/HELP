using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    public GameObject player;
    Transform moving;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    // Use this for initialization
    void Start()
    {
        moving = player.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, moving.position, 3f * Time.deltaTime);
        transform.Translate(0, 0, -1);
        Vector3 CameraPosition = transform.position;
       
    }
}
