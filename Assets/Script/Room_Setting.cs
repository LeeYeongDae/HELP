using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Setting : MonoBehaviour
{
    [SerializeField]
    public GameObject[] roomtype;
    List<int> roomList = new List<int>();

    [SerializeField]
    public GameObject[] item;
    void Awake()
    {
        for (int i = 0; i < 7;)
        {
            int currnum = Random.Range(0, 7);

            if (roomList.Contains(currnum))
            {
                currnum = Random.Range(0, 7);
            }
            else
            {
                roomList.Add(currnum);
                i++;
            }
        }
        for (int i = 0; i < 7; i++)
        {
            switch (roomList[i])
            {
                case 0:
                    Instantiate(roomtype[i], new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                    break;
                case 1:
                    Instantiate(roomtype[i], new Vector3(0f, -30f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                    break;
                case 2:
                    Instantiate(roomtype[i], new Vector3(0f, -60f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                    break;
                case 3:
                    Instantiate(roomtype[i], new Vector3(148f, 10f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                    break;
                case 4:
                    Instantiate(roomtype[i], new Vector3(148f, -20f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                    break;
                case 5:
                    Instantiate(roomtype[i], new Vector3(148f, -50f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                    break;
                case 6:
                    if (i == 0)
                        Instantiate(roomtype[i], new Vector3(52f, -24f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                    else
                        Instantiate(roomtype[i+6], new Vector3(52f, -24f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                    break; 
                //case 7:
                //    if (i == 0)
                //        Instantiate(roomtype[i], new Vector3(52f, -50f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                //    else
                //        Instantiate(roomtype[i+7], new Vector3(52f, -50f, 0f), Quaternion.Euler(0f, 0f, 0f)).transform.parent = this.gameObject.transform;
                //    break; 
                default:
                    break;
            }
        }
       }

    // Update is called once per frame
    void Update()
    {
        if (InGameManager.nextday)
            for(int i = 0; i < item.Length; i++)
            {
                Instantiate(item[i], item[i].transform.position, Quaternion.identity);
            }

    }
}
