using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Monedero : MonoBehaviour
{
    public int NumberOfCoins {get; private set;}

    public UnityEvent<Monedero> OnCoinCollected;
    public void CoinsCollected()
    {
        NumberOfCoins++;
        OnCoinCollected.Invoke(this);
    }
}
