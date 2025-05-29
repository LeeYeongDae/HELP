using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GuardPath : MonoBehaviour
{
    public static GuardPath Instance { get; private set; }

    InGameManager gamemanager;

    public GameObject CurPin;

    public Vector2 startPos;

    private Vector2 currPosition, destiPos;
    public float speed = 10f;
    public bool isFixd = false; //목적지 도착 시 이동 X
    public GameObject Destination;
    public GameObject player;


    public bool onWorkGuard;
    public int state;
    public GameObject[] FreeToEat;
    public GameObject[] WorkToEat;
    public GameObject[] FreeToWork;
    int i = 0, j = 4;
    GameObject sight;

    
    void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<InGameManager>();
        this.gameObject.transform.position = CurPin.transform.position;
        sight = this.transform.GetChild(1).gameObject;
        this.state = 0;
    }

    void Update()
    {
        startPos = gameObject.transform.position;
        player = sight.GetComponent<GuardSearch>().Player;
        
        if ((Vector2)CurPin.transform.position == startPos)
        {
            if (onWorkGuard)
            {
                if (gamemanager.dutyTime == 2)
                {
                    if (this.state == 0)
                    {
                        isFixd = false;
                        Destination = FreeToEat[i];
                        if ((Vector2)Destination.transform.position == startPos) i++;
                        if (i >= FreeToEat.Length)
                        {
                            this.state = 2;
                            i = 0;
                        }
                    }
                    else if (this.state == 1)
                    {
                        isFixd = false;
                        Destination = WorkToEat[i];
                        if ((Vector2)Destination.transform.position == startPos) i++;
                        if (i >= WorkToEat.Length)
                        {
                            this.state = 2;
                            i = 0;
                        }
                    }
                    else if (this.state == 2)
                    {
                        isFixd = true;
                        Destination = CurPin.gameObject.GetComponent<PathPin>().GetNextPin();
                    }
                }
                else if (gamemanager.dutyTime == 1)
                {
                    if (this.state == 0)
                    {
                        isFixd = false;
                        Destination = FreeToWork[i];
                        if ((Vector2)Destination.transform.position == startPos) i++;
                        if (i >= FreeToWork.Length)
                        {
                            this.state = 1;
                            i = 0;
                        }
                    }
                    else if (this.state == 2)
                    {
                        isFixd = false;
                        Destination = WorkToEat[j];
                        if ((Vector2)Destination.transform.position == startPos) j--;
                        if (j < 0 && !isFixd)
                        {
                            this.state = 1;
                            j = WorkToEat.Length -1;
                        }
                    }
                    else if (this.state == 1)
                    {
                        isFixd = true;
                        Destination = CurPin.gameObject.GetComponent<PathPin>().GetNextPin();

                    }
                }
                else if (gamemanager.dutyTime == 0)
                {
                    if (this.state == 2)
                    {
                        isFixd = false;
                        Destination = FreeToEat[j];
                        if ((Vector2)Destination.transform.position == startPos) j--;
                        if (j < 0 && !isFixd)
                        {
                            this.state = 0;
                            j = FreeToEat.Length -1;
                        }
                    }
                    else if (this.state == 0)
                    {
                        isFixd = true;
                        Destination = CurPin;
                    }
                }
            }
            else Destination = CurPin.gameObject.GetComponent<PathPin>().GetNextPin();
            destiPos = Destination.transform.position;
        }

        GuardMove();
        GuardRotate();
    }

    void FixedUpdate()
    {
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Path")
            CurPin = collision.gameObject;
    }

    void GuardMove()
    {
        currPosition = startPos;
        float walk = speed * Time.deltaTime;
        if(!isFixd)
            this.transform.position = Vector2.MoveTowards(currPosition, destiPos, walk);
        if (currPosition == destiPos)
        {
            CurPin = Destination;
        }
    }

    void GuardRotate()
    {
        Vector3 dir = destiPos - currPosition;
        Vector3 qut = Quaternion.Euler(0, 0, -90) * dir;
        if (player != null)
        {
            FacePlayer();
        }
        else
        {
            Quaternion rot = Quaternion.LookRotation(forward: Vector3.forward, upwards: qut);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, Time.deltaTime * 200f);
        }
    }
    void FacePlayer()
    {
        Vector3 dir = (Vector2)player.transform.position - currPosition;
        Vector3 qut = Quaternion.Euler(0, 0, -90) * dir;
        float distance = Vector3.Distance(player.transform.position, transform.position);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, qut, distance-0.5f, LayerMask.GetMask("Wall"));
        if (rayHit.collider == null)
        {
            Quaternion rot = Quaternion.LookRotation(forward: Vector3.forward, upwards: qut);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, Time.deltaTime * 50f);
        }
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle + 180f, Vector3.forward);
    }
}