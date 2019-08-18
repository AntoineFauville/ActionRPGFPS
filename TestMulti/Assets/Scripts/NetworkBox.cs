using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBox : Photon.MonoBehaviour
{
    private Vector3 _realPosition;
    private Quaternion _realQuaternion;

    void Update()
    {
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, _realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, _realQuaternion, 0.1f);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Network player, receive data
            _realPosition = (Vector3)stream.ReceiveNext();
            _realQuaternion = (Quaternion)stream.ReceiveNext();
        }
    }
}
