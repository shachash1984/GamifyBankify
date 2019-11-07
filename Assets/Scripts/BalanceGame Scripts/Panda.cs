using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panda : MonoBehaviour {

    static public Panda S;
    [SerializeField] private ParticleSystem _dustEffect;

    void Awake()
    {
        S = this;
        
    }
    
    public IEnumerator PlayerParticleEffect()
    {
        _dustEffect.gameObject.SetActive(true);
        yield return new WaitUntil(() => !_dustEffect.isEmitting);
        _dustEffect.gameObject.SetActive(false);
    }

}
