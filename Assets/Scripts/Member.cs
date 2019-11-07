using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Member", menuName = "Data Assets/Member", order = 1)]
public class Member : ScriptableObject{

    public string Name;
    public float Score;
    public float OriginalDebt;
    public float CurrentDebt;
    public bool AcceptedChallenge;
}
