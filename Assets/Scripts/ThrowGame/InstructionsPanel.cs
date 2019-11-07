using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsPanel : MonoBehaviour {

    private ThrowPenguin _throwPenguin;

    private void Awake()
    {
        _throwPenguin = FindObjectOfType<ThrowPenguin>();
    }

    public void CloseInstructionsPanel()
    {
        _throwPenguin.howToPlay.SetActive(false);
    }
}
