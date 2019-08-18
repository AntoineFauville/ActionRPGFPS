using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickableStates
{
    CanBePickedUp = 0,
    CantBePickedUp = 1
}

public class PickUpThrow : MonoBehaviour
{
    [SerializeField] private GameObject _objectLocation;
    [SerializeField] private float throwForce = 10f;

    private bool _carryObject;

    private PhotonView photonView;

    void Update()
    {
        if (Input.GetKeyDown("e") && !_carryObject)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.distance < 3)
                {
                    if (hit.collider.gameObject.GetComponent<PhotonView>() != null)
                    {
                        if (hit.collider.gameObject.GetComponent<PickableObject>() != null)
                        {
                            photonView = hit.collider.gameObject.GetComponent<PhotonView>();
                            GrabObject(photonView);
                        }
                        else
                        {
                            Debug.Log("Can't be picked up");
                        }
                    }
                }
            }
        }
        else if (Input.GetKeyDown("e") && _carryObject)
        {
            ReleaseObject();
        }

        if (Input.GetMouseButtonDown(0) && _carryObject)
        {
            ThrowObject();
        }

        if (_carryObject)
        {
            photonView.transform.position = _objectLocation.transform.position;
        }
    }

    void GrabObject(PhotonView photonView)
    {
        _carryObject = true;
        photonView.RPC("SwitchState", PhotonTargets.All, PickableStates.CantBePickedUp);
        //photonView.transform.SetParent(_objectLocation.transform);
        //photonView.transform.position = _objectLocation.transform.position;
    }

    void ThrowObject()
    {
        _carryObject = false;
        //photonView.transform.parent = null;
        photonView.RPC("SwitchState", PhotonTargets.All, PickableStates.CanBePickedUp);
        photonView.GetComponent<Rigidbody>().AddForce(this.transform.forward * throwForce);
    }

    void ReleaseObject()
    {
        _carryObject = false;
        //photonView.transform.parent = null;
        photonView.RPC("SwitchState", PhotonTargets.All, PickableStates.CanBePickedUp);
    }
}
