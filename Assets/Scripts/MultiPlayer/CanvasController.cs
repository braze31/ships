using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CanvasController : MonoBehaviour, IOnEventCallback
{
    private List<PlayerControls> players = new List<PlayerControls>();
    private double lastTickTime;
    
    public void AddPlayer(PlayerControls player)
    {
        players.Add(player);
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(PhotonNetwork.Time > lastTickTime + 1 &&
            PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Vector2Int[] pos = players
                .OrderBy(p => p.photonView.Owner.ActorNumber)
                .Select(p => p.Position)
                .ToArray();


            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(40, pos, options, sendOptions);

            PerformTick(pos);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        // do something?

        switch (photonEvent.Code)
        {
            case 40:
                Vector2Int[] pos = (Vector2Int[])photonEvent.CustomData;
                PerformTick(pos);
                break;
        }
    }
    private void PerformTick(Vector2Int[] pos)
    {
        if (players.Count != pos.Length) return;

        foreach (var player in players.OrderBy(p=>p.photonView.Owner.ActorNumber))
        {
            player.InstEnemyShipCanvas();
        }

        lastTickTime = PhotonNetwork.Time;
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

}
