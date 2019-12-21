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

public class Wolrd : MonoBehaviour, IOnEventCallback
{
    private LoadBalancingClient loadBalancingClient;
    private int randomG = 0;
    private double lastTickTime;
    private List<PlayerControls> players = new List<PlayerControls>();
    private bool ready;

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
            case 40:
                Debug.Log("HAPPEN");

                var p = photonEvent.Parameters;
                ExitGames.Client.Photon.Hashtable a = (ExitGames.Client.Photon.Hashtable)p[245];
                var t = a["my"];
                giveMeINT((int)t);
                break;
        }
    }

    private void giveMeINT(int x)
    {
        foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
        {
            player.takeThisNumberAndLOG(x);
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
            if (!ready)
            {
                StartCoroutine(PerformTick());
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();
                evData["my"] = Random.Range(0, 10);
                PhotonNetwork.RaiseEvent(40, evData, options, sendOptions);
            }
            
        }

    }

    private IEnumerator PerformTick()
    {
        ready = true;
        yield return new WaitForSeconds(4f);
        //randomG = Random.Range(0, 10);
        //Debug.Log(randomG);
        ready = false;
    }
}
