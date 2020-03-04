using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    private Texture2D image;
    public GameObject PrefabCanvas;
    public GameObject Rocket;
    public GameObject Laser;
    public Vector2Int Position;
    public int HowMuchRocketInSystem = 3;

    private GameObject Target;
    GameObject playerCanvas;
    private float currResPlayer;
    private Image iconCard;
    private int costCardEvent;
    private bool playerLOSE;
    private Ship ship;

    [SerializeField]
    private List<Rocket> RocketsSystem;
    [SerializeField]
    private Dictionary<int,Rocket> RocketsIDs = new Dictionary<int, Rocket>();

    private GameObject yourPing;
    private int TimeID;
    // PRSPAWN FOR RESET PREFAB ICON CARD
    //private GameObject PRSPAWN;

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        photonView = GetComponent<PhotonView>();
        PrefabCanvas.name = $"{photonView.ViewID} ID# - CanvasForEnemyPlayerName";
        playerCanvas = Instantiate(PrefabCanvas);
        yourPing = GameObject.FindGameObjectWithTag("ping");
        gameObject.name = $"{photonView.ViewID} - PlayerName";
        // add to canvascontroller all players which connected this room.
        FindObjectOfType<Wolrd>().AddPlayer(this);
        ship = GameObject.Find("Ship-Player-1").GetComponent<Ship>();
    }

    // CREATE if on event FROM self player
    // CREATE if on event FROM another player
    public void CreatePreFabForSystemRocket(string targetName, int timeID, string playerName)
    {
        if (photonView.IsMine)
        {
            GameObject pShip = GameObject.FindGameObjectWithTag(playerName);
            Image[] fALL = pShip.GetComponentsInChildren<Image>();
            foreach (var item in fALL)
            {
                if (targetName == item.name)
                {
                    item.tag = "SlotGunFull";
                    StartCoroutine(INSTrocketBytimeNtimes(item, pShip, timeID));
                    // GameConstants.TIME_ROCKET_SYSTEM-GameConstants.TIME_BEETWEN_ROCKET_SPAWN because first rocket live in first moment live system.
                    item.GetComponent<DropZone>().healthBar.EnableImageAndStartReduceHp(GameConstants.TIME_ROCKET_SYSTEM-GameConstants.TIME_BEETWEN_ROCKET_SPAWN);
                    StartCoroutine(ResetSlotDeleteIcon(item)); //  PRSPAWN.GetComponent<RawImage>()
                }
            }
        }
    }

    public void CreatePreFabForSystemLaser(string targetName, int timeID, string playerName)
    {
        if (photonView.IsMine)
        {
            GameObject pShip = GameObject.FindGameObjectWithTag(playerName);
            Image[] fALL = pShip.GetComponentsInChildren<Image>();
            foreach (var item in fALL)
            {
                if (targetName == item.name)
                {
                    item.tag = "SlotGunFull";
                    StartCoroutine(INSTlaserBytimeNtimes(item, pShip, timeID, 9f));
                    item.GetComponent<DropZone>().healthBar.EnableImageAndStartReduceHp(3f);
                    //StartCoroutine(ResetSlotDeleteIcon(item, PRSPAWN.GetComponent<RawImage>()));
                }
            }
        }
    }

    IEnumerator INSTrocketBytimeNtimes(Image item, GameObject enemyShip,int timeID)
    {
        //item.color = new Color(item.color.r, item.color.g, item.color.b, 255f);

        for (int k = 0; k < HowMuchRocketInSystem; k++)
        {
            if (item.GetComponent<DropZone>().healthBar.bar.fillAmount <= 0)
            {
                StopCoroutine("INSTrocketBytimeNtimes");
                //item.color = new Color(item.color.r, item.color.g, item.color.b, 0f);
                yield return null;
            }
            else
            {
                Vector3 pointToTravel = item.gameObject.GetComponentInChildren<DropZone>().posForR.GetComponent<RectTransform>().localPosition;
                //Vector3 pointToTravel = item.GetComponent<RectTransform>().localPosition;
                GameObject myNewS = Instantiate(Rocket, pointToTravel, Quaternion.Euler(0f, 0f, 0f));
                Image i = myNewS.GetComponentInChildren<Image>().GetComponent<Image>();
                RawImage ri = myNewS.GetComponentInChildren<RawImage>().GetComponent<RawImage>();

                item.sprite = i.sprite;

                //PRSPAWN = item.gameObject.GetComponent<DropZone>().posForR.gameObject.GetComponentInChildren<RawImage>().gameObject;
                ////item.GetComponent<RectTransform>().rotation = new Quaternion(0f, 0f, 0f, 1f);
                //PRSPAWN.GetComponent<RawImage>().texture = ri.texture;
                //PRSPAWN.GetComponent<RawImage>().color = new Color(ri.color.r, ri.color.g, ri.color.b, 0f);

                // need anchor?
                //myNewS.GetComponent<RectTransform>().anchoredPosition = new Vector3(pointToTravel.x,pointToTravel.y,pointToTravel.z);

                Rocket r = myNewS.GetComponent<Rocket>();
                RocketsSystem.Add(r);

                r.countRSystem = timeID + k;

                myNewS.transform.SetParent(enemyShip.transform, false);

                RocketsIDs[myNewS.GetInstanceID()] = r;
                yield return new WaitForSeconds(GameConstants.TIME_BEETWEN_ROCKET_SPAWN);
            }
        }
        //item.transform.parent.Find(nameSlot).tag = "SlotGun";
        item.GetComponent<DropZone>().healthBar.gameObject.GetComponent<Canvas>().enabled = false;
    }

    IEnumerator INSTlaserBytimeNtimes(Image item, GameObject enemyShip, int timeID, float TimeLifeLaser)
    {
        for (int k = 0; k < HowMuchRocketInSystem; k++)
        {
            //if (item.GetComponent<DropZone>().healthBar.bar.fillAmount <= 0)
            //{
            //    yield return null;
            //}
            //else
            //{
            //Vector3 pointToTravel = item.gameObject.GetComponentInChildren<DropZone>().posForR.GetComponent<RectTransform>().localPosition;
            GameObject myNewSystemL1 = Instantiate(Laser, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));
            GameObject myNewSystemL2 = Instantiate(Laser, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));

            IEnumerable<Image> i = myNewSystemL1.GetComponentsInChildren<Image>().Where(a => a.name == "Icon");

            item.sprite = i.First().GetComponent<Image>().sprite;

            myNewSystemL1.transform.SetParent(enemyShip.transform, true);
            myNewSystemL2.transform.SetParent(enemyShip.transform, true);
            myNewSystemL1.GetComponent<Laser>().TakeStartPosition(item.gameObject.GetComponentInChildren<DropZone>().posForR.transform.position);
            myNewSystemL2.GetComponent<Laser>().TakeStartPositionOFFLaser(item.gameObject.GetComponentInChildren<DropZone>().posForR.transform.position);
            myNewSystemL1.GetComponent<Laser>().TakeStartHPBARLaser(item);
            myNewSystemL2.GetComponent<Laser>().TakeStartHPBARLaser(item);
            //myNewSystemL1.GetComponent<Laser>().TimeLife(TimeLifeLaser);
            //myNewSystemL2.GetComponent<Laser>().TimeLife(TimeLifeLaser);
            //}
            //item.transform.parent.Find(nameSlot).tag = "SlotGun";

            yield return new WaitForSeconds(1f);
        }

        item.GetComponent<DropZone>().healthBar.gameObject.GetComponent<Canvas>().enabled = false;
    }


    IEnumerator ResetSlotDeleteIcon(Image icon)
    {
        yield return new WaitForSeconds(GameConstants.TIME_ROCKET_SYSTEM);
        //icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        //Ricon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        icon.tag = "SlotGun";
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(image);
            stream.SendNext(Position);
        }
        else
        {
            image = (Texture2D)stream.ReceiveNext();
            Position = (Vector2Int)stream.ReceiveNext();
        }
    }

    void LOSEorWIN(string status)
    {
        Canvas canvasPlayer = GameObject.FindGameObjectWithTag("PlayerCanvas").GetComponent<Canvas>();
        canvasPlayer.enabled = false;
        Image quit = GameObject.Find("quitButton").GetComponent<Image>();
        quit.enabled = true;
        Image leave = GameObject.Find("LeaveButtonA").GetComponent<Image>();
        leave.enabled = true;
        GameObject shipImage1 = GameObject.Find("Ship-Player (1)");
        shipImage1.SetActive(false);
        GameObject shipImage2 = GameObject.Find("Ship-Player (2)");
        shipImage2.SetActive(false);
        Text youLose = GameObject.FindGameObjectWithTag(status).GetComponent<Text>();
        youLose.enabled = true;
    }

    public void YouLose(int ID)
    {
        if (photonView.ViewID == ID)
        {
            playerLOSE = true;
            if (photonView.IsMine)
            {
                LOSEorWIN("Lose");
            }
        }
    }

    public void YouWin(int ID)
    {
        if (photonView.ViewID == ID)
        {
            playerLOSE = true;
            if (photonView.IsMine)
            {
                LOSEorWIN("Win");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && image != null)
        {
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();

            evData["guns"] = "Rocket";
            evData["iconCard"] = iconCard.sprite.name;

            evData["playerID"] = photonView.ViewID;
            evData["slot"] = Target.name;
            evData["tagSlot"] = Target.tag;
            evData["resor"] = currResPlayer;
            evData["TimeID"] = TimeID;

            PhotonNetwork.RaiseEvent(1, evData, options, sendOptions);
            image = null;
        }

        if (photonView.IsMine && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (!playerLOSE)
            {
                playerCanvas.GetComponentInChildren<Canvas>().GetComponent<Canvas>().enabled = true;
                playerCanvas.GetComponent<PlayerStats>().enabled = true;
            }
            if (ship.DamageDone)
            {
                checkHP();
                ship.DamageDone = false;
            }

        }
        yourPing.GetComponent<Text>().text = "Ping: " + PhotonNetwork.GetPing().ToString();
    }

    public void TriggerActivateP1(int id)
    {
        if (RocketsIDs.ContainsKey(id))
        {
            Rocket r = RocketsIDs[id];
            r.CheckForEvent = true;
        }
    }

    public void TakeDestroyRocket(int id)
    {
        foreach (var item in RocketsIDs)
        {
            Debug.Log("destroy!");
            if(item.Value.countRSystem == id)
            {
                item.Value.EXPLOYD();
            }
        }
    }

    public void ChangeTagRocket(int idRocketS)
    {
        if (RocketsIDs.ContainsKey(idRocketS))
        {
            Rocket r = RocketsIDs[idRocketS];
            r.tag = "RocketS";
        }
    }

    void checkHP()
    {
        if (ship!=null)
        {
            float hp = ship.GetComponent<Ship>().curHealth;
            //Debug.Log(hp + " " + photonView.ViewID);

            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            ExitGames.Client.Photon.Hashtable healthData = new ExitGames.Client.Photon.Hashtable();

            healthData["playerID"] = photonView.ViewID;
            healthData["hp"] = hp;

            PhotonNetwork.RaiseEvent(2, healthData, options, sendOptions);
        }
    }

    void OnEnable()
    {
        //subscribe to event
        DropZone.OnSelectedEvent += SelectAction;
    }

    void OnDisable()
    {
        //Un-subscribe to event
        DropZone.OnSelectedEvent -= SelectAction;
    }
    //This will be called when invoked
    //This method event for object card which droped on table.
    void SelectAction(GameObject target, GameObject cardStats, float currRes, Image iconGun, int timeID)
    {
        TimeID = timeID;
        Target = target;
        currResPlayer = currRes;
        iconCard = iconGun;
        Card d = cardStats.GetComponent<Card>();
        int t = d.CardInfo.Index;
        costCardEvent = Convert.ToInt32(d.Cost.text);
        playerCanvas.GetComponent<PlayerStats>().RemoveResources((float)costCardEvent, t);
        image = target.GetComponent<Image>().sprite.texture;
    }
}
