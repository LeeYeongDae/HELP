using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;
    public GameObject interObj;
    public static bool nightguard = false;

    public Text GameOverTxt;
    public Text DutyTxT;
    public Text TimeTxT;
    [SerializeField]
    private Slider SusSlider;
    [SerializeField]
    private Image sliderImg;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3[] spawnPoints;
    [SerializeField] private EventManager eventManager;

    public int GuardOnDuty;

    public static bool nextday;

    public static bool isClear;
    public static bool isOver;

    public static bool allSlept;

    public int LifeCount;
    public static float susLv;


    float inTime = 0, min = 0;
    int hour;
    bool pm;

    public int dutyTime;    // 0 = Free, 1 = Work, 2 = Eat, 3 = Patrol
    public int guardOnJob;
    public enum Duty
    { Free, Work, Eat, Patrol }
    public static InGameManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject("GameManager").AddComponent<InGameManager>();
            return instance;
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        GameObject go = GameObject.Find("GameManager");
        if (go == null)
        {
            go = new GameObject { name = "GameManager" };
            go.AddComponent<InGameManager>();
        }
        DontDestroyOnLoad(go);
        instance = go.GetComponent<InGameManager>();

        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        hour = 7;
        pm = false;

        susLv = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOver) GameOver();
        if (isClear) GameClear();

        if (allSlept)
        {
            inTime = 1410f;
            hour = 6;
            min = 30f;
            allSlept = false;
        }

        inTime += 3f * Time.deltaTime;
        if (inTime >= 930f) nightguard = true;
        else if (inTime >= 900f) dutyTime = (int)Duty.Patrol;  //22시~ 수면시간
        else if (inTime >= 720f) dutyTime = (int)Duty.Free;  //19~22시 자유시간
        else if (inTime >= 660f) dutyTime = (int)Duty.Eat;  //18~19시 식사시간
        else if (inTime >= 480f) dutyTime = (int)Duty.Work;  //15~18시 작업시간
        else if (inTime >= 360f) dutyTime = (int)Duty.Free;  //13~15시 자유시간
        else if (inTime >= 300f) dutyTime = (int)Duty.Eat;  //12~13시 식사시간
        else if (inTime >= 120f) dutyTime = (int)Duty.Work;  //9~12시 작업시간
        else if (inTime >= 60f) dutyTime = (int)Duty.Eat;   //8~9시 식사시간
        else if (inTime >= 0f) dutyTime = (int)Duty.Free;  //7~8시 자유시간

        if (inTime >= 1440f)
        {
            nextday = inTime == 1440f ? true : false;
            nightguard = false;
            inTime -= 1440f;
        }
        
        GetDuty();
        GetTime();
        GetSusLv();
    }

    public void Init()
    {
        
    }
    public void GetDuty()
    {
        switch(dutyTime)
        {
            case 0:
                DutyTxT.text = string.Format("자유시간");
                break;
            case 1:
                DutyTxT.text = string.Format("업무시간");
                break;
            case 2:
                DutyTxT.text = string.Format("식사시간");
                break;
            case 3:
                DutyTxT.text = string.Format("수면시간");
                break;
            default:
                break;
        }
    }
    public void GetTime()
    {
        min += 3f * Time.deltaTime;
        if (min >= 60f)
        {
            hour += 1;
            min -= 60;
        }
        if (hour >= 12)
        {
            pm = !pm;
            hour -= 12;
        }

        if (pm)
        {
            if (hour == 0)
                TimeTxT.text = string.Format("PM {0:D2}:{1:D2}", hour+12, (int)min);
            else
                TimeTxT.text = string.Format("PM {0:D2}:{1:D2}", hour, (int)min);
        }
        else
        {
            TimeTxT.text = string.Format("AM {0:D2}:{1:D2}", hour, (int)min);
        }
    }

    public void GetSusLv()
    {
        SusSlider.value = susLv;

        if (susLv >= 100f)
            isOver = true;

        if(SusSlider.value == 0f)
        {
            sliderImg.color = new Color32(0, 0, 0, 0);
        }
        else if (SusSlider.value <= 40f)
        {
            sliderImg.color = new Color32(0, 255, 0, 200);
        }
        else if (SusSlider.value <= 60f)
        {
            sliderImg.color = new Color32(255, 255, 0, 200);
        }
        else if (SusSlider.value <= 80f)
        {
            sliderImg.color = new Color32(255, 127, 0, 200);
        }
        else
        {
            sliderImg.color = new Color32(255, 0, 0, 200);
        }
    }

    void GameOver()
    {
        GameOverTxt.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void GameClear()
    {
        GameOverTxt.text = "Clear";
        GameOverTxt.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // 씬 로딩 완료 후 호출될 콜백 등록
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
        }
    }

    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId))
        {
            var playerInstance = Instantiate(playerPrefab, GetStartPositionForClient(clientId), Quaternion.identity);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true); // true는 ownership 부여
            playerInstance.GetComponent<PlayableChr>().eventManager = eventManager;
        }
    }
    private Vector3 GetStartPositionForClient(ulong clientId)
    {
        int index = (int)(clientId % (ulong)spawnPoints.Length);
        return spawnPoints[index];
    }
}
