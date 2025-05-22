using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using LobbyRelay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using System.Collections;

public class PlayableChr : NetworkBehaviour
{
    public LobbyPlayerData LPD;
    public Joystick joy;
    public EventManager eventManager;
    [SerializeField]
    TMP_Text m_nameOutput = default;
    Camera m_mainCamera;
    NetworkVariable<Vector3> net_position = new NetworkVariable<Vector3>(Vector3.zero);
    
    Action<ulong, Action<PlayerData>> m_retrieveName;

    List<Item> m_inven;

    Rigidbody2D rigid;
    Vector2 moveVec;
    Vector3 dirView = Vector3.down;
    public float speed;
    float p_status;
    public bool isRun;
    public bool isSlow;
    public bool isInteracting;
    public bool isLInteract;
    public bool isRInteract;
    public int useItemId;
    public bool onBreaking;

    public int warnMode;    //아이템 의심레벨 합계
    public Tilemap interTile;
    public Vector3Int tilePos;
    public TileBase hitTile;
    public GameObject interObject;

    [SerializeField] private Vector3[] spawnPoints;
    [SerializeField] private Camera playerCamera;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }


    private Vector3 GetStartPositionForClient(ulong clientId)
    {
        int index = (int)(clientId % (ulong)spawnPoints.Length);
        return spawnPoints[index];
    }
    public override void OnNetworkSpawn()
    {
        
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            
            // UI 연결 등

            //m_renderer.color = Color.cyan; // 예시
            //m_nameOutput.text = $"Player {OwnerClientId}";
        }
        else
        {
            joy = null;
        }

        if (IsServer)
        {
            this.transform.position = GetStartPositionForClient(OwnerClientId);
        }
    }

    
    void Start()
    {
        warnMode = 0;

        if (OwnerClientId == 0)
            this.transform.position = new Vector3(-12, 51);


        if (IsOwner)
        {

            string myId = AuthenticationService.Instance.PlayerId;
            string myName = LobbyPlayerData.Instance.GetPlayerNameById(myId);
            SetNameServerRpc(myName);

            playerCamera.enabled = true;
            playerCamera.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            playerCamera.enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;

        
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        joy = eventManager.joy;
        GameObject.Find("Inventory").GetComponent<Inventory>().SetLocalPlayer(this);
        m_inven = GameObject.Find("Inventory").GetComponent<Inventory>().items;

        if (isRun)
        {
            if (isSlow)
                p_status = 1.15f;
            else p_status = 1.5f;
        }
        else
        {
            if (isSlow)
                p_status = 0.75f;
            else p_status = 1f;
        }
        Interact();
        Interact_StateCheck();
        Run_StateCheck();
        CalculateLevel();
        PlayerMove();
    }

    void Run_StateCheck()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
    }

    void Interact_StateCheck()
    {
        if(Input.GetKey(KeyCode.E) || eventManager.GetpressedInter())  
            isInteracting = true;
        else 
            isInteracting = false;


        if(Input.GetKey(KeyCode.Alpha1) || eventManager.GetLpressed())
        {
            isLInteract = true;
            useItemId = GameObject.Find("L_Hand").GetComponent<Use_Item>().UseItem();
        }
        else
        {
            isLInteract = false;
        }


        if (Input.GetKey(KeyCode.Alpha2) || eventManager.GetRpressed())
        {
            isRInteract = true;
            useItemId = GameObject.Find("R_Hand").GetComponent<Use_Item>().UseItem();
        }
        else
        {
            isRInteract = false;
        }
        
        if(!eventManager.GetLpressed() && !eventManager.GetRpressed())
        {
            useItemId = 0;
        }
    }

    void PlayerMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveVec = new Vector2(x, y) * speed * p_status * Time.deltaTime;
        if (joy != null && joy.Direction != Vector2.zero)
            moveVec = joy.Direction * speed * p_status * Time.deltaTime;
        this.rigid.MovePosition(rigid.position + moveVec);
        dirView = Quaternion.Euler(0, 0, 0) * moveVec;
        net_position.Value = this.transform.position;
    }
    void Interact()
    {
        Debug.DrawRay(rigid.position, dirView * 1f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirView, 1f, LayerMask.GetMask("Interactable"));

        if (rayHit.collider != null)
        {
            interObject = rayHit.collider.gameObject;
            if(interObject.TryGetComponent<Tilemap>(out interTile))
            {
                tilePos = interTile.WorldToCell(new Vector3(rayHit.point.x - (rayHit.normal.x * 0.01f), rayHit.point.y - (rayHit.normal.y * 0.01f), 0));

                hitTile = interTile.GetTile(interTile.WorldToCell(new Vector3(rayHit.point.x - (rayHit.normal.x * 0.01f), rayHit.point.y - (rayHit.normal.y * 0.01f), 0)));
            }
        }
        else
        {
            interObject = null;
            interTile = null;
            hitTile = null;
        }
    }

    void CalculateLevel()
    {
        int warn = 0;
        foreach (var item in m_inven)
        {
            warn += item.sus_Level;
        }
        warnMode = warn;
    }

    public void DoInteraction()
    {
        //PressIBt = eventManager.Getpressed();
    }

    public Tilemap GetTilemap()
    {
        return interTile;
    }

    public TileBase GethitTile()
    {
        return hitTile;
    }

    public Vector3Int gettilePos()
    {
        return tilePos;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Finish"))
        {
            InGameManager.isClear = true;
        }
    }

    [ServerRpc]
    void SetNameServerRpc(string playerName)
    {
        SetNameClientRpc(playerName);
    }

    [ClientRpc]
    void SetNameClientRpc(string playerName)
    {
        m_nameOutput.text = playerName;
    }
}
