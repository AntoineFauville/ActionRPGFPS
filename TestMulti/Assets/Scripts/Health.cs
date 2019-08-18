using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 100;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}
