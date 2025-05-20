using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int index;
    [SerializeField] Image image;
    private Item _item;
    public Item item
    {
        get { return _item; }   //������ ������ ���� ����
        set
        {
            _item = value;      //item ���� ����
            if (_item != null )
            {
                this.gameObject.SetActive(true);
                image.sprite = item.itemImage;
                image.color = new Color( 1, 1, 1, 1);
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
                this.gameObject.SetActive(false);
            }
        }
    }
    public Text count;
    public int quantity;

    private void Update()
    {
        if (this.quantity == 0) this.gameObject.SetActive(false);
    }

    public bool Pickedup { get; private set; }

}
