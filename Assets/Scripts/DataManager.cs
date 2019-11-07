using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserName { Jona, Ester, Shahar}

public class DataManager : MonoBehaviour {

    static public DataManager S;
    public User currentUser;
    public Challenge[] challenges;
    public Member[] members;


    void Awake()
    {
        if (S != null)
            Destroy(this);
        S = this;
    }
}
