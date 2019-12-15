using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    //public GameObject PrefabCanvas;
    //[SerializeField]
    //public static GameObject PlayerCanvasP1;
    //[SerializeField]
    //public static GameObject PlayerCanvasP2;

    private new PhotonView photonView;

    private void Start()
    {
        Vector3 pos = new Vector3(0, 0);

        GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        photonView = player.GetComponent<PhotonView>();
        player.name = $"{photonView.ViewID} - PlayerName";
        //GameObject CanvasP1 = FindObjectInChilds(player, "HUD - Canvas");
        //GameObject found = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy")).Find(g => g.transform.IsChildOf(this.transform));

        //var CanvasP1 = GetAllObjectsInScene();
        PhotonPeer.RegisterType(typeof(Vector2Int), 242, SerializeVector2Int, DeserializeVector2Int);
        PhotonPeer.RegisterType(typeof(Texture2D), 242, SerializeImage, DeserializeImage);
    }

    //public GameObject GetAllObjectsInScene()
    //{
    //    foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
    //    {
    //        if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
    //            continue;

    //        //if (!EditorUtility.IsPersistent(go.transform.root.gameObject))
    //        //    continue;

    //        if (go.tag == "PlayerCanvas")
    //        {
    //            return go;
    //        }
    //    }
    //    return null;
    //}


    public static GameObject FindObjectInChilds(GameObject gameObject, string gameObjectName)
    {
        Canvas[] children = gameObject.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas item in children)
        {
            if (item.name == gameObjectName)
            {
                return item.gameObject;
            }
        }

        return null;
    }

    void Update()
    {

    }

    public static object DeserializeVector2Int(byte[] data)
    {
        Vector2Int res = new Vector2Int();
        res.x = BitConverter.ToInt32(data, 0);
        res.y = BitConverter.ToInt32(data, 4);

        return res;
    }

    public static byte[] SerializeVector2Int(object obj)
    {
        Vector2Int vec = (Vector2Int)obj;
        byte[] res = new byte[8];

        BitConverter.GetBytes(vec.x).CopyTo(res, 0);
        BitConverter.GetBytes(vec.y).CopyTo(res, 4);

        return res;
    }


    [SerializeField]
    private static Texture2D tex;
    private static Texture2D DeserializeImage(byte[] data)
    {
        tex.LoadImage(data);
        return tex;
    }
    private static byte[] SerializeImage(object obj)
    {
        Texture2D tex = new Texture2D(30, 30, TextureFormat.RGB24, false);
        tex.Apply();
        tex.Compress(false);

        Texture2D decopmpresseTex = tex.DeCompress();

        var bytes = decopmpresseTex.EncodeToPNG();
        return bytes;
    }




    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }
}
