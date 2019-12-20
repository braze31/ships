using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    private RawImage shipEnemy;
    private Texture2D image;
    public GameObject PrefabCanvas;
    public GameObject Rocket;
    public Transform Trans;

    [SerializeField]
    public static Canvas PlayerCanvasP1;
    [SerializeField]
    public static Canvas PlayerCanvasP2;

    public Transform EnemyShip;
    public Vector2Int Position;

    private GameObject Target;

    // need sync this function for all players =/

    [PunRPC]
    public void InstEnemyShipCanvas()
    {
        GameObject go = GameManager.FindObjectInChilds(gameObject, "HUD");

        Image[] fALL = go.GetComponentsInChildren<Image>();

        
        foreach (var item in fALL)
        {
            if (item.name == Target.name)
            {
                Vector3 pointToTravel = item.GetComponent<RectTransform>().localPosition;
                //Debug.Log(pointToTravel);
                GameObject myNewS = Instantiate(Rocket, pointToTravel, Quaternion.Euler(0f, 0f, -180f));
                //myNewS.GetComponent<RectTransform>().anchoredPosition = new Vector3(pointToTravel.x,pointToTravel.y,pointToTravel.z);
                myNewS.transform.SetParent(go.transform, false);
            }
        }
    }

    
    // Start is called before the first frame update

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
        GameObject playerCanvas = Instantiate(PrefabCanvas);
        gameObject.name = $"{photonView.ViewID} - PlayerName";
        //example find
        //var taggedObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g => g.tag == "PlayerCanvas").ToList();
        //shipEnemy = taggedObjects[0].GetComponent<RawImage>();

        var CanvasP1 = GetAllObjectsInScene();
        if (photonView.IsMine)
        {
            playerCanvas.GetComponentInChildren<Canvas>().GetComponent<Canvas>().enabled = true;
        }
        if (!photonView.IsMine)
        {
            gameObject.GetComponentInChildren<Canvas>().GetComponent<Canvas>().enabled = true;
        }

        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //{
        //    Debug.Log(CanvasP1.name + $" {photonView.ViewID}");
        //    PlayerCanvasP1 = gameObject.GetComponentInChildren<Canvas>();
        //}
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        //{
        //    Debug.Log(CanvasP1.name + $" {photonView.ViewID}");
        //    PlayerCanvasP2 = gameObject.GetComponentInChildren<Canvas>(); 
        //}

        // add to canvascontroller all players which connected this room.
        FindObjectOfType<CanvasController>().AddPlayer(this);
    }

    public GameObject GetAllObjectsInScene()
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                continue;

            //if (!EditorUtility.IsPersistent(go.transform.root.gameObject))
            //    continue;

            if (go.tag == "PlayerCanvas")
            {
                return go;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && image != null)
        {
            InstEnemyShipCanvas();
            image = null;
        }
        //if (!photonView.IsMine)
        //{
        //    PlayerCanvasP1.GetComponent<Canvas>().enabled = true;
        //}
        //if (photonView.IsMine)
        //{
        //    PlayerCanvasP2.GetComponent<Canvas>().enabled = false;
        //}
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
    //This method sync object card which droped on table with OTHER player.
    void SelectAction(GameObject target)
    {
        image = target.GetComponent<Image>().sprite.texture;
        photonView.RPC("InstEnemyShipCanvas", RpcTarget.Others);
        Target = target;
    }

}
