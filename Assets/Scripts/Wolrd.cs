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
    public Text waitCanvas;
    private bool twiceHPcheck;

    public struct MyRocket
    {
        public int trig;
        public int IDR;
    }

    private Dictionary<int, MyRocket> allRockets = new Dictionary<int, MyRocket>();
    private bool maxPlayerWasBeen;

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

                var pID1 = pararmsRocketSystem["playerID"];
                var trig = pararmsRocketSystem["trig"];
                var idR = pararmsRocketSystem["idRocket"];
                var countRSystem = pararmsRocketSystem["countRSystem"];
                var exploydEvent = pararmsRocketSystem["exploydEvent"];
                var targetRocket = pararmsRocketSystem["tag"];

                MyRocket mr = new MyRocket();
                mr.trig = (int)trig;
                mr.IDR = (int)idR;

                //Debug.Log((int)countRSystem + " " + (int)trig + " " + (string)pID1);
                if ((int)trig!=-1)
                {
                    if (!allRockets.ContainsKey((int)countRSystem))
                    {
                        allRockets.Add((int)countRSystem, mr);
                    }
                    else
                    {
                        if (allRockets[(int)countRSystem].trig != (int)trig)
                        {
                            int IDR1 = allRockets[(int)countRSystem].IDR;
                            takeTwoValue(mr.IDR, IDR1);
                            allRockets.Remove((int)countRSystem);
                        }
                    }
                }

                if ((bool)exploydEvent)
                {
                    exploydRocketbyID((int)countRSystem);
                }

                //if ((string)targetRocket == "RocketS")
                //{
                //    AllPlayerTagRocketS((int)idR);
                //}
                break;
            case 4:
                var p4 = photonEvent.Parameters;
                ExitGames.Client.Photon.Hashtable numberAndbool = (ExitGames.Client.Photon.Hashtable)p4[245];
                //var boolForRandomNumberAndEvent = numberAndbool["boolForRandomNumberAndEvent"];


                if (numberAndbool["randomNumber"] != null)
                {
                    var randomNumber = numberAndbool["randomNumber"];
                    GivePlayersSameRandomNumber((int)randomNumber);
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

        var timeID = hashPlayer["TimeID"];
        if ((string)icon == "rocket")
        {
            foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
            {
                if (player.photonView.ViewID != (int)pID)
                {
                    player.CreatePreFabForSystemRocket((string)slot, (int)timeID, "Enemy");
                }
                if (player.photonView.ViewID == (int)pID)
                {
                    player.CreatePreFabForSystemRocket((string)slot, (int)timeID, "Player1");
                }
            }
        }
        if ((string)icon == "laser")
        {
            foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
            {
                if (player.photonView.ViewID != (int)pID)
                {
                    player.CreatePreFabForSystemLaser((string)slot, (int)timeID, "Enemy");
                }
                if (player.photonView.ViewID == (int)pID)
                {
                    player.CreatePreFabForSystemLaser((string)slot, (int)timeID, "Player1");
                }
            }
        }
        if ((string)icon == "shield")
        {
            foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
            {
                if (player.photonView.ViewID != (int)pID)
                {
                    player.CreatePreFabForSystemShield((string)slot, (int)timeID, "Enemy");
                }
                if (player.photonView.ViewID == (int)pID)
                {
                    player.CreatePreFabForSystemShield((string)slot, (int)timeID, "Player1");
                }
            }
        }
    }

    public void GivePlayersSameRandomNumber(int randomNumber)
    {
        foreach (var player in players)
        {
            player.TakeRandomNumer(randomNumber);
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

    public void takeTwoValue(int trig0, int trig1)
    {
        foreach (var player in players)
        {
            player.TriggerActivateP1(trig0);
            player.TriggerActivateP1(trig1);
        }
    }

    //public void AllPlayerTagRocketS(int idRocketWithTagRocketS)
    //{
    //    foreach (var player in players.OrderBy(p => p.photonView.Owner.ActorNumber))
    //    {
    //        player.ChangeTagRocket(idRocketWithTagRocketS);
    //    }
    //}

    public void exploydRocketbyID(int countRSystem)
    {
        foreach (var player in players)
        {
            player.TakeDestroyRocket(countRSystem);
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
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.CurrentRoom != null)
        {
            waitCanvas.GetComponentInChildren<Text>().text = "";
            maxPlayerWasBeen = true;
        }
        else
        {
            if (maxPlayerWasBeen)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
    }
}
