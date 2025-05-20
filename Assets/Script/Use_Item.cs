using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Use_Item : MonoBehaviour
{
    Inventory inven;
    GameObject L_Hand, R_Hand, text;
    PlayableChr player;
    Image img;
    public int L_ItemId, R_ItemId;


    // Start is called before the first frame update
    void Start()
    {
        inven = GameObject.Find("Inventory").GetComponent<Inventory>();
        img = transform.GetChild(0).GetComponent<Image>();
        text = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        L_Hand = inven.L_Hand;
        R_Hand = inven.R_Hand;

        EquipItem();
        UseItem();
    }

    void EquipItem()
    {

        if (this.gameObject.name == "L_Hand")
        {
            if(L_Hand == null || !L_Hand.activeSelf)
            {
                img.sprite = null;
                text.SetActive(true);
                L_ItemId = 0;
                inven.L_Hand = null;
                return;
            }
            img.sprite = L_Hand.GetComponent<Image>().sprite;
            L_ItemId = L_Hand.GetComponent<Slot>().item.itemId;
            text.SetActive(false);
        }
        else if (this.gameObject.name == "R_Hand")
        {
            if (R_Hand == null || !R_Hand.activeSelf)
            {
                img.sprite = null;
                text.SetActive(true);
                R_ItemId = 0;
                inven.R_Hand = null;
                return;
            }
            img.sprite = R_Hand.GetComponent<Image>().sprite;
            R_ItemId = R_Hand.GetComponent<Slot>().item.itemId;
            text.SetActive(false);
        }
    }

    public void UnequipItem()
    {
        if (L_Hand == null)
        {
            img.sprite = null;
            text.SetActive(true);
            L_ItemId = 0;
            return;
        }
        if (R_Hand == null)
        {
            img.sprite = null;
            text.SetActive(true);
            R_ItemId = 0;
            return;
        }
    }

        public int UseItem()
    {
        if (this.gameObject.name == "L_Hand" && inven.FindItem(L_ItemId) != null) return L_ItemId;
        else if (this.gameObject.name == "R_Hand" && inven.FindItem(R_ItemId) != null) return R_ItemId;
        else return 0;
    }
}
