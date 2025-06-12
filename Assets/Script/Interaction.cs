using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public interface IObjectItem
{
    Item Obitem();
}

public class Interaction : MonoBehaviour, IObjectItem
{
    Inventory inven;

    GameObject playerObj;
    PlayableChr player;
    public Tilemap interTile;
    TileBase hitTile;
    public Vector3Int tilePos;

    public GameObject minigame;
    bool onMinigame;
    Tilemap door;

    public Slider workGage;
    [SerializeField]
    public Item obtem;

    void Awake()
    {
        inven = GameObject.Find("Inventory").GetComponent<Inventory>();
        door = GameObject.Find("Door").GetComponent<Tilemap>();
    }

    void Start()
    {
        playerObj = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if(playerObj.GetComponent<PlayableChr>().IsOwner) player = playerObj.GetComponent<PlayableChr>();
        interTile = player.GetTilemap();
        hitTile = player.GethitTile();
        tilePos = player.gettilePos();
        if (minigame != null && minigame.GetComponent<MiniGame>().isSuccess)
        {
            door.SetTile(tilePos, null);
            minigame.GetComponent<MiniGame>().isSuccess = false;
        }
    }

    bool GetHand()
    {
        if (player.isLInteract || player.isRInteract)
        {
            return true; 
        }
        else return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (this.gameObject.tag == "Item")
                workGage.gameObject.SetActive(true);
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (this.gameObject.tag == "Item" && player.isInteracting)
            {
                workGage.value += Time.deltaTime;
                if (!inven.invenFull && workGage.value == workGage.maxValue)
                    this.ObtainItem(obtem);
            }
            if (this.gameObject.tag == "Bed" && InGameManager.nightguard)
            {
                InGameManager.allSlept = true;
            }
                
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (GetHand() && player.useItemId == 101 && this.gameObject.name == "BreakableWall" && !player.onBreaking)
            {
                try
                {
                    interTile.SetTile(tilePos, null);
                    player.onBreaking = true;
                }
                catch
                {
                    StartCoroutine(WaitForIt());
                    return;
                }
                int rand = new System.Random().Next(3);
                if(rand < 1) inven.RemoveItem(inven.FindItem(101));
                StartCoroutine(WaitForIt());
                player.onBreaking = false;
            }
            if (GetHand() && player.useItemId == 102 && this.gameObject.name == "BreakableDirt" && !player.onBreaking)
            {
                try
                {
                    interTile.SetTile(tilePos, null);
                    player.onBreaking = true;
                }
                catch
                {
                    StartCoroutine(WaitForIt());
                    return;
                }
                int rand = new System.Random().Next(5);
                if (rand < 2) inven.RemoveItem(inven.FindItem(102));
                StartCoroutine(WaitForIt());
                player.onBreaking = false;
            }
            if (GetHand() && player.useItemId == 103 && this.gameObject.name == "CraftTable")
            {
                try
                {
                    inven.AddItem(obtem);
                }
                catch
                {
                    inven.FreshSlot();
                    StartCoroutine(WaitForIt());
                    return;
                }
                inven.RemoveItem(inven.FindItem(103));
                inven.FreshSlot();
                StartCoroutine(WaitForIt());
            }
            if (GetHand() && player.useItemId == 104 && this.gameObject.name == "Door")
            {
                try
                {
                    OnMiniGame();
                }
                catch
                {
                    StartCoroutine(WaitForIt());
                    return;
                }
                inven.RemoveItem(inven.FindItem(104));
                StartCoroutine(WaitForIt());
            }
            if (GetHand() && player.useItemId == 105 && this.gameObject.name == "PrisonDoor")
            {
                try
                {
                    interTile.SetTile(tilePos, null);
                }
                catch
                {
                    StartCoroutine(WaitForIt());
                    return;
                }
                StartCoroutine(WaitForIt());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (this.gameObject.tag == "Item")
            {
                workGage.gameObject.SetActive(false);
                workGage.value = 0;
            }
        }
    }

    void OnMiniGame()
    {
        minigame.GetComponent<MiniGame>().isSuccess = false;
        minigame.SetActive(true);
        minigame.GetComponent<MiniGame>().OnPicking = true;
        onMinigame = true;
        
    }

    public void ObtainItem(Item item)
    {
        inven.AddItem(item);
        this.gameObject.SetActive(false);   //Arrest 되기 전까지 재획득 불가능
    }

    public Item Obitem()
    {
        return this.obtem;
    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(0.1f);
        player.onBreaking = false;
    }
}
