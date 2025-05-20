using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public int itemId;
    public string itemName;
    public Sprite itemImage;
    public int sus_Level;
    public int itemSize;
    public int itemCount;
    public string itemDesc;
}
