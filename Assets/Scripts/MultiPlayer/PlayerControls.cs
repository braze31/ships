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
    public Transform Trans;
    public Vector2Int Position;

    private GameObject Target;
    GameObject playerCanvas;
    private float currResPlayer;
    private Image iconCard;
    private int costCardEvent;
    private bool playerLOSE;
    private GameObject ship;
    // Start is called before the first frame update


    public void ChangeEnemyShipContent(string guns, string targetName, string nameCard, string tag)
    {
        if (photonView.IsMine)
        {
            if (guns == Rocket.name)
            {
                GameObject enemyShip = GameObject.FindGameObjectWithTag("Enemy");
                Image[] fALL = enemyShip.GetComponentsInChildren<Image>();
                foreach (var item in fALL)
                {
                    if (targetName == item.name)
                    {
                        item.tag = "SlotGunFull";
                        StartCoroutine(INSTrocketBytimeNtimes(item,enemyShip));
                        
                    }
                }
            }
        }
    }

    IEnumerator INSTrocketBytimeNtimes(Image item, GameObject enemyShip)
    {
        for (int k = 0; k < 4; k++)
        {
            Vector3 pointToTravel = item.GetComponent<RectTransform>().localPosition;
            GameObject myNewS = Instantiate(Rocket, pointToTravel, Quaternion.Euler(0f, 0f, 0f));
            Image i = myNewS.GetComponentInChildren<Image>().GetComponent<Image>();
            //Debug.Log(i.sprite.name);
            //Debug.Log(nameCard);
            // create Content for EnemyShip, Card event
            //if (nameCard == i.sprite.name)
            //{
                item.color = new Color(item.color.r, item.color.g, item.color.b, 255f);
                item.sprite = i.sprite;
                StartCoroutine(ResetSlotDeleteIcon(item));
            //}
            // need anchor?
            //myNewS.GetComponent<RectTransform>().anchoredPosition = new Vector3(pointToTravel.x,pointToTravel.y,pointToTravel.z);
            myNewS.transform.SetParent(enemyShip.transform, false);
            yield return new WaitForSeconds(1.5f);
            
            //StartCoroutine(INSTrocketBytimeNtimes(item, nameCard, enemyShip));
        }
        item.tag = "SlotGun";
    }


    IEnumerator ResetSlotDeleteIcon(Image icon)
    {
        yield return new WaitForSeconds(3);
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        //icon.tag = "SlotGun";
    }

    public void CreatePreFab(string targetName)
    {
        if (photonView.IsMine)
        {
            GameObject pShip = GameObject.FindGameObjectWithTag("Player1");
            Image[] fALL = pShip.GetComponentsInChildren<Image>();
            foreach (var item in fALL)
            {
                if (targetName == item.name)
                {
                    StartCoroutine(INSTrocketBytimeNtimes(item, pShip));
                }
            }
        }
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

    public void YouLose(int ID)
    {
        if (photonView.ViewID == ID)
        {
            playerLOSE = true;
            if (photonView.IsMine)
            {
                Canvas canvasPlayer = GameObject.FindGameObjectWithTag("PlayerCanvas").GetComponent<Canvas>();
                canvasPlayer.enabled = false;

                Text youLose = GameObject.FindGameObjectWithTag("Lose").GetComponent<Text>();
                youLose.enabled = true;
                Button quit = GameObject.Find("quitButton").GetComponent<Button>();
                quit.gameObject.SetActive(true);
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
                Canvas canvasPlayer = GameObject.FindGameObjectWithTag("PlayerCanvas").GetComponent<Canvas>();
                canvasPlayer.enabled = false;

                Text youLose = GameObject.FindGameObjectWithTag("Win").GetComponent<Text>();
                youLose.enabled = true;
                Button quit = GameObject.Find("quitButton").GetComponent<Button>();
                quit.gameObject.SetActive(true);
            }
        }
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        PrefabCanvas.name = $"ID# {photonView.ViewID} - CanvasForEnemyPlayerName";
        playerCanvas = Instantiate(PrefabCanvas);
        gameObject.name = $"{photonView.ViewID} - PlayerName";

        // add to canvascontroller all players which connected this room.
        FindObjectOfType<Wolrd>().AddPlayer(this);
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
            ship = GameObject.Find("Ship-Player");

            checkHP();

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
    void SelectAction(GameObject target, GameObject cardStats, float currRes, Image iconGun)
    {
        CreatePreFab(target.name);
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
