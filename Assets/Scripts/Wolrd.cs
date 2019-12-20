using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Random = UnityEngine.Random;
using Photon.Pun;
using System.Linq;

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
                giveMeINT();
                break;
        }
    }

    private void giveMeINT()
    {
        foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
        {
            player.takeThisNumberAndLOG(randomG);
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
                PhotonNetwork.RaiseEvent(40, randomG, options, sendOptions);
            }
            
        }

    }

    private IEnumerator PerformTick()
    {
        ready = true;
        yield return new WaitForSeconds(4f);
        randomG = Random.Range(0, 10);
        //Debug.Log(randomG);
        ready = false;
    }
}
