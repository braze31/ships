using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Random = UnityEngine.Random;
using Photon.Pun;
using System.Linq;
using ExitGames.Client.Photon;
using System.Text;
using System;
using UnityEngine.UI;

public class Wolrd : MonoBehaviour, IOnEventCallback
{
    private LoadBalancingClient loadBalancingClient;
    private List<PlayerControls> players = new List<PlayerControls>();
    public Canvas waitCanvas;
    public void AddPlayer(PlayerControls player)
    {
        players.Add(player);
    }

    public Wolrd()
    {
        this.loadBalancingClient = new LoadBalancingClient();
        this.SubscribeToCallbacks();
    }

    ~Wolrd()
    {
        this.UnsubscribeFromCallbacks();
    }

    private void SubscribeToCallbacks()
    {
        this.loadBalancingClient.AddCallbackTarget(this);
    }

    private void UnsubscribeFromCallbacks()
    {
        this.loadBalancingClient.RemoveCallbackTarget(this);
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 1:
                Debug.Log("1 HAPPEN");
                var p1 = photonEvent.Parameters;

                //comment for check parametrs
                //var lines1 = p1.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
                //string a1 = string.Join(Environment.NewLine, lines1);

                //Debug.Log(a1);

                ExitGames.Client.Photon.Hashtable parametrsEventShip = (ExitGames.Client.Photon.Hashtable)p1[245];
                ApplyCommand(parametrsEventShip);

                break;
        }
    }

    public void ApplyCommand(ExitGames.Client.Photon.Hashtable hashPlayer)
    {
        // parametrs ships
        var pID = hashPlayer["playerID"];
        var g = hashPlayer["guns"];
        var slot = hashPlayer["slot"];
        var icon = hashPlayer["iconCard"];
        foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
        {
            if (player.photonView.ViewID != (int)pID)
            {
                player.ChangeEnemyShipContent((string)g, (string)slot, (string)icon);
            }
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // do something master
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            waitCanvas.GetComponentInChildren<Text>().text = "";
        }
    }
}
