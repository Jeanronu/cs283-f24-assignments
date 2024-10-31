using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonederoUI : MonoBehaviour
{
    private TextMeshProUGUI coinText;

    // Start is called before the first frame update
    void Start()
    {
        coinText = GetComponent<TextMeshProUGUI>();
    }

    public void updateCoinText(Monedero monedero)
    {
        coinText.text = monedero.NumberOfCoins.ToString();
    }
}
