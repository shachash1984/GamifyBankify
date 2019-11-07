using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ServerResponse
{
    public bool success;
    public string error;
    public int gameIndex;
    public string PIN;
    public string userContainerJSON;
}

[System.Serializable]
public class Challenge  {

    public bool success;
    public string error;
    public int gameIndex;
    public string PIN;
    public UserContainer userContainer;
}

[System.Serializable]
public class UserContainer
{
    [SerializeField] public List<User> users = new List<User>();
}
