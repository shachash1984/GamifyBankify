using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {

    private int _id;
    private string _name;
    private bool _isHost = false;

    public int GetID()
    {
        return _id;
    }

    public void SetID(int newID)
    {
        _id = newID;
    }

    public string GetName()
    {
        return _name;
    }

    public void SetName(string newName)
    {
        _name = newName;
    }

    public bool IsHost()
    {
        return _isHost;
    }

    public void SetHost(bool host)
    {
        _isHost = host;
    }

}
