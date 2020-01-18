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
    private Dictionary<int, float> hpPlayers = new Dictionary<int, float>();
    public Canvas waitCanvas;
    private bool twiceHPcheck;

    private Dictionary<int, int> RocketTrigRightPlayer1 = new Dictionary<int,int>();
    private Dictionary<int, int> RocketTrigRightPlayer2 = new Dictionary<int, int>();


    public void AddPlayer(PlayerControls player)
    {
        players.Add(player);
        hpPlayers[player.photonView.ViewID] = 1.35f;
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
            // case 1 for check parametrs Cards events for ship
            case 1:
                //Debug.Log("1 HAPPEN");
                var p1 = photonEvent.Parameters;

                //comment for check parametrs
                //var lines1 = p1.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
                //string a1 = string.Join(Environment.NewLine, lines1);

                //Debug.Log(a1);

                ExitGames.Client.Photon.Hashtable parametrsEventShip = (ExitGames.Client.Photon.Hashtable)p1[245];
                ApplyCommand(parametrsEventShip);

                break;
            // case 2 for check HP players
            case 2:
                //Debug.Log("2 HAPPEN");
                var p2 = photonEvent.Parameters;
                ExitGames.Client.Photon.Hashtable healthEventShip = (ExitGames.Client.Photon.Hashtable)p2[245];

                //var lines1 = p2.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
                //string a1 = string.Join(Environment.NewLine, lines1);
                //Debug.Log(a1);

                var pID = healthEventShip["playerID"];
                var hp = healthEventShip["hp"];
                //Debug.Log(pID + " " + hp);

                hpPlayers[(int)pID] = (float)hp;

                if ((float)hp<0)
                {
                    if (!twiceHPcheck)
                    {
                        ApplyHealtBar((int)pID);
                        twiceHPcheck = true;
                    }
                }
                break;
            // case 3 for check trigger where rocket place in world position on each player.
            case 3:
                //Debug.Log("3 HAPPEN");
                var p3 = photonEvent.Parameters;
                ExitGames.Client.Photon.Hashtable pararmsRocketSystem = (ExitGames.Client.Photon.Hashtable)p3[245];

                //var lines1 = p3.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
                //string a1 = string.Join(Environment.NewLine, lines1);
                //Debug.Log(a1);

                var pID1 = pararmsRocketSystem["playerID"];
                var trig = pararmsRocketSystem["trig"];
                var idR = pararmsRocketSystem["idRocket"];


                if ((int)trig != -1)
                {
                    //Debug.Log(pID1 + " " + $"{trig}" + " " + $"{idR}");

                    if ((int)trig == 1)
                    {
                        RocketTrigRightPlayer1[(int)pID1] = (int)idR;
                    }
                    
                    if ((int)trig == 0)
                    {
                        RocketTrigRightPlayer2[(int)pID1] = (int)idR;
                    }
                }

                if (RocketTrigRightPlayer1.Count == 2 && RocketTrigRightPlayer2.Count == 2)
                {
                    DauRazreshenie(RocketTrigRightPlayer1, RocketTrigRightPlayer2);
                }

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
        var tagTarget = hashPlayer["tagSlot"];
        foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
        {
            if (player.photonView.ViewID != (int)pID)
            {
                player.ChangeEnemyShipContent((string)g, (string)slot, (string)icon, (string) tagTarget);
            }
        }
    }

    public void ApplyHealtBar(int pID)
    {
        foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
        {
            if (player.photonView.ViewID != (int)pID)
            {
                player.YouWin(player.photonView.ViewID);
            }
            else
            {
                player.YouLose(player.photonView.ViewID);
            }
        }
    }
    public void DauRazreshenie(Dictionary<int, int> rocketTrigRightPlayer1, Dictionary<int, int> rocketTrigRightPlayer2)
    {
        foreach (var player in players)
        {
            Debug.Log(rocketTrigRightPlayer1[player.photonView.ViewID] + " is razrehenie" );
            player.TriggerActivateP1(rocketTrigRightPlayer1[player.photonView.ViewID]);

            player.TriggerActivateP1(rocketTrigRightPlayer2[player.photonView.ViewID]);
            

            RocketTrigRightPlayer1 = new Dictionary<int, int>();
            RocketTrigRightPlayer2 = new Dictionary<int, int>();
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
