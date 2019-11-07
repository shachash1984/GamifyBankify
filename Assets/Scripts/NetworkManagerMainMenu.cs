using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManagerMainMenu : NetworkBehaviour {

	public int portNumber = 22;
	public string serverIPAddress = "95.85.3.77";

	public void HostGame() {
		NetworkManager.singleton.networkPort = portNumber;
		NetworkLobbyManager.singleton.StartServer ();
		//NetworkManager.singleton.StartHost ();

		//GameObject.Find ("Manager").GetComponent<ManagerReferences> ().mainMenu.SetActive (false);
	}


	public void JoinGame() {
		NetworkManager.singleton.networkPort = portNumber;
		NetworkManager.singleton.networkAddress = serverIPAddress;
		NetworkLobbyManager.singleton.StartClient ();


		//GameObject.Find ("Manager").GetComponent<ManagerReferences> ().mainMenu.SetActive (false);
		//GameObject.Find ("Manager").GetComponent<ManagerReferences> ().buttonDisconnect.SetActive (true);
	}

	public void DisconnectClient() {
		NetworkLobbyManager.singleton.StopClient ();
		//GameObject.Find ("Manager").GetComponent<ManagerReferences> ().buttonDisconnect.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		if (isMainServer ()) {
			HostGame ();
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool isMainServer() {
		return SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
	}
}
