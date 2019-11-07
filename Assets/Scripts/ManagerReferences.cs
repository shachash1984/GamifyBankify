using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ManagerReferences : NetworkBehaviour {

    public string serverAddress = "https://dawntaylorgames.com";
    public User user;
	public GameObject localPlayer;
	public GameObject mainMenu;
	public GameObject buttonDisconnect;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }



}
