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

    // need sync this function for all players =/
    public void InstEnemyShipCanvas()
    {
        var myNewS = Instantiate(Rocket, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        myNewS.transform.parent = PlayerCanvasP1.transform;
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
        // why 2 player doesn't have this name???
        PrefabCanvas.name = $"ID# {photonView.ViewID} - CanvasForEnemyPlayerName";
        GameObject playerCanvas = Instantiate(PrefabCanvas);

        //example find
        //var taggedObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g => g.tag == "PlayerCanvas").ToList();
        //shipEnemy = taggedObjects[0].GetComponent<RawImage>();

        var CanvasP1 = GetAllObjectsInScene();
        if (photonView.IsMine)
        {
            playerCanvas.GetComponentInChildren<Canvas>().GetComponent<Canvas>().enabled = true;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log(CanvasP1.name + $" {photonView.ViewID}");
            PlayerCanvasP1 = gameObject.GetComponentInChildren<Canvas>();
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log(CanvasP1.name + $" {photonView.ViewID}");
            PlayerCanvasP2 = gameObject.GetComponentInChildren<Canvas>(); 
        }

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
        if (!photonView.IsMine)
        {
            PlayerCanvasP1.GetComponent<Canvas>().enabled = true;
        }
        if (photonView.IsMine)
        {
            PlayerCanvasP2.GetComponent<Canvas>().enabled = false;
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
    void SelectAction(GameObject target)
    {
        image = target.GetComponent<Image>().sprite.texture;
        Debug.Log(image.name);
    }

}
