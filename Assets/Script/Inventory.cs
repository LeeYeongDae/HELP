using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public List<Item> items;

    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private Slot[] slots;
    public bool invenFull;
    public GameObject Handy;
    GameObject SelectItemObj;
    public GameObject L_Hand, R_Hand;
    int Objindex, Lindex, Rindex;

    [SerializeField] private PlayableChr localPlayer;

    public void SetLocalPlayer(PlayableChr player)
    {
        localPlayer = player;
        Debug.Log("���� �÷��̾� ���� �Ϸ�");
    }

    public PlayableChr GetLocalPlayer()
    {
        return localPlayer;
    }

#if UNITY_EDITOR
    private void OnValidate()       //Bag ������ slot �ڵ� ���
    {
        slots = slotParent.GetComponentsInChildren<Slot>();
    }
#endif

    void Awake()
    {
        FreshSlot();                //���� �� items�� ������ �κ��丮 �ֱ�
    }

    void Update()
    {

        if (items.Count >= slots.Length)
            invenFull = true;
        else
            invenFull &= !invenFull;
    }

    public void FreshSlot()
    {
        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].item = items[i];
            if (slots[i].quantity == 0)
            { 
                slots[i].quantity++;
                slots[i].count.gameObject.SetActive(true);
                slots[i].count.text = slots[i].quantity.ToString();
            }
        }
        for (; i < slots.Length; i++)
        {
            slots[i].item = null;
            if (slots[i].quantity == 0) slots[i].count.gameObject.SetActive(false);
        }
    }

    public void AddItem(Item _item)
    {
        if (localPlayer == null) return;


        if (_item.itemSize == 0)
        {
            Slot slotStack = GetItemStack(_item);
            if (slotStack != null)
            {
                slotStack.quantity++;
                slotStack.count.text = slotStack.quantity.ToString();
            }
            else if (items.Count < slots.Length)
            {
                items.Add(_item);
                FreshSlot();
            }
        }
        else if (items.Count < slots.Length)
        {
            items.Add(_item);
            FreshSlot();
        }
        else
        {
            print("������ ���� á���ϴ�.");
        } 
    }
    Slot GetItemStack(Item _item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == _item && slots[i].quantity < 20)
                return slots[i];
        }

        return null;
    }

    public void RemoveItem(Item _item)
    {
        if (localPlayer == null) return;


        int delIndex = 0;
        Slot slotStack = GetItemStack(_item);
        slotStack.quantity--;
        slotStack.count.text = slotStack.quantity.ToString();
        if(slotStack.quantity == 0)
        {
            if (L_Hand == slotStack.gameObject)
            {
                if(Lindex < Rindex)
                {
                    R_Hand = FindSlot(Rindex - 1).gameObject;
                    Rindex--;
                    delIndex = Rindex;
                }
                L_Hand = null;
                Lindex = 0;
            }
            else if (R_Hand == slotStack.gameObject)
            {
                if(Rindex < Lindex)
                {
                    L_Hand = FindSlot(Lindex - 1).gameObject;
                    Lindex--;
                    delIndex = Lindex;
                }
                R_Hand = null;
                Rindex = 0;
            }
            items.Remove(_item);
            for(int i=delIndex-1;i<slots.Length-1;i++)
            {
                if (slots[i].quantity == 0 && slots[i + 1].quantity == 0) break;
                slots[i].quantity = slots[i + 1].quantity;
                slots[i].count.text = slots[i].quantity.ToString();
            }
        }
        FreshSlot();
    }


    public Item FindItem(int id)
    {
        foreach (var item in items)
        {
            if (item.itemId == id)
                return item;
        }
        return null;
    }

    public Slot FindSlot(int index)
    {
        foreach (var slot in slots)
        {
            if (slot.index == index)
                return slot;
        }
        return null;
    }

    public void OpenHand()
    {
        SelectItemObj = EventSystem.current.currentSelectedGameObject;
        Objindex = SelectItemObj.GetComponent<Slot>().index;
        Handy.SetActive(true);
    }

    public void PickItem(int whichhand)
    {
        if (whichhand == 0)
        {
            if (SelectItemObj == L_Hand)
            {
                L_Hand = null;
                Lindex = 0;
            }
            else if (SelectItemObj == R_Hand)
            {
                R_Hand = null;
                Rindex = 0;
                L_Hand = SelectItemObj;
                Lindex = Objindex;
            }
            else
            {
                L_Hand = SelectItemObj;
                Lindex = Objindex;
            }
        }
        else if (whichhand == 1)
            if (SelectItemObj == R_Hand)
            {
                R_Hand = null;
                Rindex = 0;
            }
            else if (SelectItemObj == L_Hand)
            {
                L_Hand = null;
                Lindex = 0;
                R_Hand = SelectItemObj;
                Rindex = Objindex;
            }
            else 
            {
                R_Hand = SelectItemObj;
                Rindex = Objindex;
            }
        
    }
    

}
