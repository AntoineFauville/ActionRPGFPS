using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private int damage = 100;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.collider.gameObject.GetComponent<PhotonView>() != null)
                {
                    PhotonView photonView;
                    photonView = hit.collider.gameObject.GetComponent<PhotonView>();
                    photonView.RPC("TakeDamage", PhotonTargets.All, damage);
                }
            }
        }
    }
}
