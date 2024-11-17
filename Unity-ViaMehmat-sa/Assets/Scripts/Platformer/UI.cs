using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Player player;
    
    public Text hitPointsText;
    public Text coinsText;
    
    private int currentHitPoints;
    private int currentCoins;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHitPoints != player.hitPoints)
        {
            currentHitPoints = player.hitPoints;
            hitPointsText.text = "HP : " + currentHitPoints.ToString();
        }

        if (currentCoins != player.coins)
        {
            currentCoins = player.coins;
            coinsText.text = "Coins : " + currentCoins.ToString();
        }

    }
}
