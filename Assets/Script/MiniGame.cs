using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
{
    bool OnminiGame;
    public bool isFail = false;
    public bool isSuccess = false;
    [Header("Lock Picking")]
    public bool OnPicking;
    public GameObject LockPicking;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        OnminiGame = false;
        slider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (OnPicking) OnminiGame = true;
        LockPicking.SetActive(OnPicking);
        if(isFail)
        {
            OnPicking = false;
            OnminiGame = false;
            isFail = false;
            this.gameObject.SetActive(false);
        }
        if (!OnPicking)
            slider.value = 0;
        if (slider.value == 100)
        {
            isSuccess = true;
            isFail = true;
        }
    }
}
