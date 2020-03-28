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
    public GameObject Shield;
    public GameObject Bomb;
    public GameObject Flare;
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
    private int randomNumber;
    private bool boolForRandomNumberAndEvent = false;

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
            IEnumerable<Image> fALL = pShip.GetComponentsInChildren<Image>().Where(a => a.gameObject.name == "ImageIcon");
            foreach (var item in fALL)
            {
                if (targetName == item.transform.parent.name)
                {
                    item.transform.parent.tag = "SlotGunFull";
                    StartCoroutine(INSTrocketBytimeNtimes(item, pShip, timeID));
                    // GameConstants.TIME_ROCKET_SYSTEM-GameConstants.TIME_BEETWEN_ROCKET_SPAWN because first rocket live in first moment live system.
                    item.transform.parent.GetComponent<DropZone>().healthBar.EnableImageAndStartReduceHp(GameConstants.TIME_ROCKET_SYSTEM-GameConstants.TIME_BEETWEN_ROCKET_SPAWN);
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
            IEnumerable<Image> fALL = pShip.GetComponentsInChildren<Image>().Where(a => a.gameObject.name == "ImageIcon");
            foreach (var item in fALL)
            {
                if (targetName == item.transform.parent.name)
                {
                    item.transform.parent.tag = "SlotGunFull";
                    StartCoroutine(INSTlaserBytimeNtimes(item, pShip, timeID));
                    item.transform.parent.GetComponent<DropZone>().healthBar.EnableImageAndStartReduceHp(3f);
                    //StartCoroutine(ResetSlotDeleteIcon(item, PRSPAWN.GetComponent<RawImage>()));
                }
            }
        }
    }

    public void CreatePreFabForSystemShield(string targetName, int timeID, string playerName)
    {
        if (photonView.IsMine)
        {
            GameObject pShip = GameObject.FindGameObjectWithTag(playerName);
            IEnumerable<Image> fALL = pShip.GetComponentsInChildren<Image>().Where(a => a.gameObject.name == "ImageIcon");
            foreach (var item in fALL)
            {
                if (targetName == item.transform.parent.name)
                {
                    item.transform.parent.tag = "SlotGunFull";
                    StartCoroutine(INSTshield(item, pShip, timeID));
                    item.transform.parent.GetComponent<DropZone>().healthBar.EnableImageAndStartReduceHp(5f);
                    //StartCoroutine(ResetSlotDeleteIcon(item, PRSPAWN.GetComponent<RawImage>()));
                }
            }
        }
    }

    public void CreatePreFabForSystemBomb(string targetName, int timeID, string playerName)
    {
        if (photonView.IsMine)
        {
            GameObject pShip = GameObject.FindGameObjectWithTag(playerName);
            IEnumerable<Image> fALL = pShip.GetComponentsInChildren<Image>().Where(a => a.gameObject.name == "ImageIcon");
            foreach (var item in fALL)
            {
                if (targetName == item.transform.parent.name)
                {
                    item.transform.parent.tag = "SlotGunFull";
                    StartCoroutine(INSTBomb(item, pShip, timeID));
                    item.transform.parent.GetComponent<DropZone>().healthBar.EnableImageAndStartReduceHp(6f);
                    //StartCoroutine(ResetSlotDeleteIcon(item, PRSPAWN.GetComponent<RawImage>()));
                }
            }
        }
    }

    public void CreatePreFabForSystemFlare(string targetName, int timeID, string playerName)
    {
        if (photonView.IsMine)
        {
            GameObject pShip = GameObject.FindGameObjectWithTag(playerName);
            IEnumerable<Image> fALL = pShip.GetComponentsInChildren<Image>().Where(a => a.gameObject.name == "ImageIcon");
            foreach (var item in fALL)
            {
                if (targetName == item.transform.parent.name)
                {
                    item.transform.parent.tag = "SlotGunFull";
                    StartCoroutine(INSTFlare(item, pShip, timeID));
                    item.transform.parent.GetComponent<DropZone>().healthBar.EnableImageAndStartReduceHp(3f);
                    //StartCoroutine(ResetSlotDeleteIcon(item, PRSPAWN.GetComponent<RawImage>()));
                }
            }
        }
    }

    IEnumerator INSTrocketBytimeNtimes(Image item, GameObject enemyShip,int timeID)
    {
        for (int k = 0; k < HowMuchRocketInSystem; k++)
        {
            //Debug.Log(item.transform.parent.GetComponent<DropZone>().healthBar.bar.fillAmount);
            if (item.transform.parent.tag == "SlotGun")
            {
                StopCoroutine(INSTrocketBytimeNtimes(item,enemyShip,timeID));
                //item.color = new Color(item.color.r, item.color.g, item.color.b, 0f);
                yield return null;
            }
            else
            {
                Vector3 pointToTravel = item.transform.parent.gameObject.GetComponentInChildren<DropZone>().posForR.GetComponent<RectTransform>().localPosition;
                //Vector3 pointToTravel = item.GetComponent<RectTransform>().localPosition;

                GameObject myNewS = Instantiate(Rocket, pointToTravel, Quaternion.Euler(0f, 0f, 0f));

                Image i = myNewS.GetComponentInChildren<Image>().GetComponent<Image>();
                RawImage ri = myNewS.GetComponentInChildren<RawImage>().GetComponent<RawImage>();

                Rocket r = myNewS.GetComponent<Rocket>();
                //give rocket bool for copy rocket with another side window.
                r.trueRocket = true;
                RocketsSystem.Add(r);

                item.transform.parent.gameObject.GetComponentInChildren<DropZone>().iconSystem.sprite = i.sprite;
                r.countRSystem = timeID + k;

                myNewS.transform.SetParent(enemyShip.transform, false);

                RocketsIDs[myNewS.GetInstanceID()] = r;
                GameObject myNewSOFF = Instantiate(Rocket, pointToTravel, Quaternion.Euler(0f, 0f, 0f));
                myNewSOFF.transform.SetParent(enemyShip.transform, false);

                if (PhotonNetwork.IsMasterClient)
                {
                    Target = enemyShip;
                    boolForRandomNumberAndEvent = true;
                }

                myNewS.GetComponent<Rocket>().TakeRandomNumberForSearch(randomNumber);
                myNewSOFF.GetComponent<Rocket>().TakeRandomNumberForSearch(randomNumber);

                yield return new WaitForSeconds(GameConstants.TIME_BEETWEN_ROCKET_SPAWN);
            }
        }
        //item.transform.parent.Find(nameSlot).tag = "SlotGun";
        item.transform.parent.GetComponent<DropZone>().healthBar.gameObject.GetComponent<Canvas>().enabled = false;
    }

    IEnumerator INSTlaserBytimeNtimes(Image item, GameObject enemyShip, int timeID)
    {
        for (int k = 0; k < HowMuchRocketInSystem; k++)
        {
            if (item.transform.parent.tag == "SlotGun")
            {
                StopCoroutine(INSTlaserBytimeNtimes(item, enemyShip, timeID));
                yield return null;
            }
            GameObject myNewSystemL1 = Instantiate(Laser, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));
            GameObject myNewSystemL2 = Instantiate(Laser, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));

            IEnumerable<Image> i = myNewSystemL1.GetComponentsInChildren<Image>().Where(a => a.name == "Icon");

            item.transform.parent.gameObject.GetComponentInChildren<DropZone>().iconSystem.sprite = i.First().GetComponent<Image>().sprite;

            myNewSystemL1.transform.SetParent(enemyShip.transform, true);
            myNewSystemL2.transform.SetParent(enemyShip.transform, true);
            myNewSystemL1.GetComponent<Laser>().TakeStartPosition(item.transform.parent.gameObject.GetComponentInChildren<DropZone>().posForR.transform.position);
            myNewSystemL2.GetComponent<Laser>().TakeStartPositionOFFLaser(item.transform.parent.gameObject.GetComponentInChildren<DropZone>().posForR.transform.position);
            myNewSystemL1.GetComponent<Laser>().TakeStartHPBARLaser(item);
            myNewSystemL2.GetComponent<Laser>().TakeStartHPBARLaser(item);

            if (PhotonNetwork.IsMasterClient)
            {
                Target = enemyShip;
                boolForRandomNumberAndEvent = true;
            }

            myNewSystemL1.GetComponent<Laser>().TakeRandomNumberForSearch(randomNumber);
            myNewSystemL2.GetComponent<Laser>().TakeRandomNumberForSearch(randomNumber);

            yield return new WaitForSeconds(1f);
        }

        item.transform.parent.GetComponent<DropZone>().healthBar.gameObject.GetComponent<Canvas>().enabled = false;
    }

    IEnumerator INSTshield(Image item, GameObject enemyShip, int timeID)
    {
        if (item.transform.parent.tag == "SlotGun")
        {
            StopCoroutine(INSTshield(item, enemyShip, timeID));
            yield return null;
        }
        enemyShip.GetComponent<Ship>().shieldActive = true;
        //RawImage shieldBar = enemyShip.GetComponent<Ship>().shieldImage;
        //Shield shield = enemyShip.GetComponent<Ship>().shieldImage.GetComponent<Shield>();

        GameObject myNewSystemShield = Instantiate(Shield, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));
        myNewSystemShield.transform.position = enemyShip.GetComponent<Ship>().ShieldPosforShip.position;
        myNewSystemShield.transform.SetParent(enemyShip.transform, true);
        enemyShip.GetComponent<Ship>().shieldImage = myNewSystemShield.GetComponent<RawImage>();

        myNewSystemShield.GetComponent<Shield>().TakeHpbarValueFromImage(item);
        IEnumerable<Image> i = enemyShip.GetComponent<Ship>().shieldImage.GetComponentsInChildren<Image>().Where(a => a.name == "Icon");
        item.transform.parent.gameObject.GetComponentInChildren<DropZone>().iconSystem.sprite = i.First().GetComponent<Image>().sprite;

        yield return new WaitForSeconds(5f);
        enemyShip.GetComponent<Ship>().shieldActive = false ;
        item.transform.parent.GetComponent<DropZone>().healthBar.gameObject.GetComponent<Canvas>().enabled = false;
    }

    IEnumerator INSTBomb(Image item, GameObject enemyShip, int timeID)
    {
        for (int k = 0; k < 2; k++)
        {
            if (item.transform.parent.tag == "SlotGun")
            {
                StopCoroutine(INSTBomb(item, enemyShip, timeID));
                yield return null;
            }
            else
            {
                Vector3 pointToTravel = item.transform.parent.gameObject.GetComponentInChildren<DropZone>().posForR.GetComponent<RectTransform>().localPosition;

                GameObject myNewS = Instantiate(Bomb, pointToTravel, Quaternion.Euler(0f, 0f, 0f));

                Image i = myNewS.GetComponentInChildren<Image>().GetComponent<Image>();
                RawImage ri = myNewS.GetComponentInChildren<RawImage>().GetComponent<RawImage>();

                Bomb b = myNewS.GetComponent<Bomb>();

                b.trueRocket = true;

                item.transform.parent.gameObject.GetComponentInChildren<DropZone>().iconSystem.sprite = i.sprite;

                myNewS.transform.SetParent(enemyShip.transform, false);

                GameObject myNewSOFF = Instantiate(Bomb, pointToTravel, Quaternion.Euler(0f, 0f, 0f));
                myNewSOFF.transform.SetParent(enemyShip.transform, false);

                if (PhotonNetwork.IsMasterClient)
                {
                    Target = enemyShip;
                    boolForRandomNumberAndEvent = true;
                }

                myNewS.GetComponent<Bomb>().TakeRandomNumberForSearch(randomNumber);
                myNewSOFF.GetComponent<Bomb>().TakeRandomNumberForSearch(randomNumber);

                yield return new WaitForSeconds(5.9f);
            }
        }
        //item.transform.parent.Find(nameSlot).tag = "SlotGun";
        item.transform.parent.GetComponent<DropZone>().healthBar.gameObject.GetComponent<Canvas>().enabled = false;
    }

    IEnumerator INSTFlare(Image item, GameObject enemyShip, int timeID)
    {
        for (int k = 0; k < 3; k++)
        {
            if (item.transform.parent.tag == "SlotGun")
            {
                StopCoroutine(INSTFlare(item, enemyShip, timeID));
                yield return null;
            }
            else
            {
                Vector3 pointToTravel = item.transform.parent.gameObject.GetComponentInChildren<DropZone>().posForR.GetComponent<RectTransform>().localPosition;

                GameObject myNewS = Instantiate(Flare, pointToTravel, Quaternion.Euler(0f, 0f, 0f));

                Image i = myNewS.GetComponentInChildren<Image>().GetComponent<Image>();
                RawImage ri = myNewS.GetComponentInChildren<RawImage>().GetComponent<RawImage>();

                Flare b = myNewS.GetComponent<Flare>();

                b.trueRocket = true;

                item.transform.parent.gameObject.GetComponentInChildren<DropZone>().iconSystem.sprite = i.sprite;

                myNewS.transform.SetParent(enemyShip.transform, false);

                GameObject myNewSOFF = Instantiate(Flare, pointToTravel, Quaternion.Euler(0f, 0f, 0f));
                myNewSOFF.transform.SetParent(enemyShip.transform, false);

                if (PhotonNetwork.IsMasterClient)
                {
                    Target = enemyShip;
                    boolForRandomNumberAndEvent = true;
                }

                myNewS.GetComponent<Flare>().TakeRandomNumberForSearch(randomNumber);
                myNewSOFF.GetComponent<Flare>().TakeRandomNumberForSearch(randomNumber);

                yield return new WaitForSeconds(0.97f);
            }
        }
        //item.transform.parent.Find(nameSlot).tag = "SlotGun";
        item.transform.parent.GetComponent<DropZone>().healthBar.gameObject.GetComponent<Canvas>().enabled = false;
    }


    // Player System search for each player same target
    public int GiveAllPlayersSameRandom(GameObject enemyShip)
    {
        // if target for rocket empty = empty system with tag equal SlotGun
        // Search target for empty slot use random number
        if (enemyShip.name == "Ship-Player-1")
        {
            GameObject go = GameObject.Find("Ship-Player-2");
            Ship ship = go.GetComponent<Ship>();
            ship.FindAllSlotGunsEmpty();
            IOrderedEnumerable<RectTransform> sortList = ship.slotGunsEmpty.ToList().OrderBy(nameS => nameS.name);
            randomNumber = UnityEngine.Random.Range(0, sortList.Count());
        }
        else
        {
            GameObject go = GameObject.Find("Ship-Player-1");
            Ship ship = go.GetComponent<Ship>();
            ship.FindAllSlotGunsEmpty();
            IOrderedEnumerable<RectTransform> sortList = ship.slotGunsEmpty.ToList().OrderBy(nameS => nameS.name);
            randomNumber = UnityEngine.Random.Range(0, sortList.Count());
        }
        return randomNumber;
    }


    IEnumerator ResetSlotDeleteIcon(Image icon)
    {
        yield return new WaitForSeconds(GameConstants.TIME_ROCKET_SYSTEM);
        //icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        //Ricon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        icon.transform.parent.tag = "SlotGun";
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
        GameObject shipImage3 = GameObject.Find("Ship-Player-1");
        shipImage3.SetActive(false);
        GameObject shipImage4 = GameObject.Find("Ship-Player-2");
        shipImage4.SetActive(false);
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

    public void TakeRandomNumer(int _randomNuber)
    {
        randomNumber = _randomNuber;
    }

    void SendEventFromPlayer()
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
    }

    void SendEventNumber()
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();

        evData["randomNumber"] = GiveAllPlayersSameRandom(Target);
        evData["boolForRandomNumberAndEvent"] = boolForRandomNumberAndEvent;
        PhotonNetwork.RaiseEvent(4, evData, options, sendOptions);
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && image != null)
        {
            SendEventFromPlayer();
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

        if (boolForRandomNumberAndEvent)
        {
            SendEventNumber();
            boolForRandomNumberAndEvent = false;
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

    //public void ChangeTagRocket(int idRocketS)
    //{
    //    if (RocketsIDs.ContainsKey(idRocketS))
    //    {
    //        Rocket r = RocketsIDs[idRocketS];
    //        r.tag = "RocketS";
    //    }
    //}

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
    void SelectAction(Image Imagetarget, GameObject cardStats, float currRes, Image iconGun, int timeID)
    {
        TimeID = timeID;
        Target = Imagetarget.gameObject.transform.parent.gameObject;
        currResPlayer = currRes;
        iconCard = iconGun;
        Card d = cardStats.GetComponent<Card>();
        int t = d.CardInfo.Index;
        costCardEvent = Convert.ToInt32(d.Cost.text);
        playerCanvas.GetComponent<PlayerStats>().RemoveResources((float)costCardEvent, t);
        image = Imagetarget.GetComponent<Image>().sprite.texture;
    }
}
