using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSearch : MonoBehaviour
{
    public int player_warnLv;
    int night_warnLv;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Player != null)
            this.player_warnLv = Player.GetComponent<PlayableChr>().warnMode;
        this.night_warnLv = InGameManager.nightguard ? 4 : 0;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            this.Player = collision.gameObject;
    
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InGameManager.susLv += (this.player_warnLv + this.night_warnLv) * 8 * Time.deltaTime;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.Player = null;
            this.player_warnLv = 0;
        }
    }
}
