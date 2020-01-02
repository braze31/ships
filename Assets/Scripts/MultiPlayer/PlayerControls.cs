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
    private int costCardEvent;
    private GameObject testRocketMove;
    public static int movespeed = 300;
    public Vector3 userDirection = Vector3.right;
    // Start is called before the first frame update

    public void ChangeEnemyShipContent(string guns, string targetName)
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
                        // create Content for EnemyShip, Card event
                        Vector3 pointToTravel = item.GetComponent<RectTransform>().localPosition;
                        GameObject myNewS = Instantiate(Rocket, pointToTravel, Quaternion.Euler(0f, 0f, 0f));
                        // need anchor?
                        //myNewS.GetComponent<RectTransform>().anchoredPosition = new Vector3(pointToTravel.x,pointToTravel.y,pointToTravel.z);
                        myNewS.transform.SetParent(enemyShip.transform, false);
                        myNewS.GetComponent<Image>().SetNativeSize();
                        testRocketMove = myNewS;
                    }
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
            evData["playerID"] = photonView.ViewID;
            evData["slot"] = Target.name;
            evData["resor"] = currResPlayer;
            PhotonNetwork.RaiseEvent(1, evData, options, sendOptions);
            image = null;
        }

        if (photonView.IsMine && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            playerCanvas.GetComponentInChildren<Canvas>().GetComponent<Canvas>().enabled = true;
            playerCanvas.GetComponent<PlayerStats>().enabled = true;
        }
        if (testRocketMove != null)
        {
            testRocketMove.transform.Translate(userDirection * movespeed * Time.deltaTime);
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
    void SelectAction(GameObject target, GameObject cardStats, float currRes)
    {
        Target = target;
        currResPlayer = currRes;
        Card d = cardStats.GetComponent<Card>();
        int t = d.CardInfo.Index;
        costCardEvent = Convert.ToInt32(d.Cost.text);
        playerCanvas.GetComponent<PlayerStats>().RemoveResources((float)costCardEvent, t);
        image = target.GetComponent<Image>().sprite.texture;
    }
}
