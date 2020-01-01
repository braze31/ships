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
    [SerializeField]
    private static Texture2D tex;
    private bool checkPlayerbool = true;
    public Canvas waitCanvas;

    private void Start()
    {
        PhotonPeer.RegisterType(typeof(Vector2Int), 242, SerializeVector2Int, DeserializeVector2Int);
        PhotonPeer.RegisterType(typeof(Texture2D), 242, SerializeImage, DeserializeImage);
    }

    void Update()
    {
        if (checkPlayerbool)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                checkPlayerbool = false;
                Vector3 pos = new Vector3(0, 0);
                GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
                waitCanvas.GetComponentInChildren<Text>().text = "";
            }
        }
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

    private static Texture2D DeserializeImage(byte[] data)
    {
        tex.LoadImage(data);
        return tex;
    }
    private static byte[] SerializeImage(object obj)
    {
        Texture2D tex = (Texture2D)obj;
        tex.Apply();
        tex.Compress(false);

        Texture2D decopmpresseTex = tex.DeCompress();

        var bytes = decopmpresseTex.EncodeToPNG();
        return bytes;
    }

    //public void Leave()
    //{
    //    PhotonNetwork.LeaveRoom();
    //}

    //public override void OnLeftRoom()
    //{
    //    SceneManager.LoadScene(0);
    //}

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }
}
