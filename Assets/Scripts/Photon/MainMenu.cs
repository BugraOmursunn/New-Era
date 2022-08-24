using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MainMenu : MonoBehaviourPunCallbacks
{
	public TMP_InputField createInput;
	public TMP_InputField joinInput;

	public void CreateRoom()
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 20;
		PhotonNetwork.CreateRoom(createInput.text, roomOptions);
	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRoom(joinInput.text);
	}
	public override void OnJoinedRoom()
	{
		PhotonNetwork.LoadLevel("BattleAreaPhoton");
	}
}
