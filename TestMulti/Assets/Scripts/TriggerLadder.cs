using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLadder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PhotonView photonView;
        photonView = other.gameObject.GetComponent<PhotonView>();
        photonView.RPC("SwitchState", PhotonTargets.All, CharacterMovingStates.Climbing);
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView photonView;
        photonView = other.gameObject.GetComponent<PhotonView>();
        photonView.RPC("SwitchState", PhotonTargets.All, CharacterMovingStates.Running);
    }
}
