using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] private int amountOfCoins;
    [SerializeField] private GameObject coin;
    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;

    void Start()
    {
        amountOfCoins = Random.Range(minCoins, maxCoins);
        int additionalOffset = amountOfCoins / 2;
        for (int i = 0; i < amountOfCoins; i++)
        {
            Vector3 offset = new Vector2 (i - additionalOffset, 0);
            Instantiate(coin, transform.position + offset, Quaternion.identity, transform);
        }
    }

  
}
