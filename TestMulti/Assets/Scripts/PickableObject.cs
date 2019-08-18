using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public PickableStates PickableState;

    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private BoxCollider _boxCollider;

    void Start()
    {
        PickableState = PickableStates.CanBePickedUp;
    }

    [PunRPC]
    void SwitchState(PickableStates pickableStates)
    {
        PickableState = pickableStates;

        switch (PickableState)
        {
            case PickableStates.CanBePickedUp:
                _boxCollider.isTrigger = false;
                _rigidBody.isKinematic = false;
                _rigidBody.useGravity = true;
                break;
            case PickableStates.CantBePickedUp:
                _boxCollider.isTrigger = true;
                _rigidBody.isKinematic = true;
                _rigidBody.useGravity = false;
                break;
        }
    }
}
