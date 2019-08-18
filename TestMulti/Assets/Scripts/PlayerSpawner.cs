using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour 
{
    [SerializeField] private GameObject SpawnPoint;

    void OnJoinedRoom()
    {
        CreatePlayerObject();
    }

    void CreatePlayerObject()
    {
        Vector3 position = new Vector3( 110f, 1.5f, 95f );

        GameObject newPlayerObject = PhotonNetwork.Instantiate("FPSView", SpawnPoint.transform.position, SpawnPoint.transform.rotation, 0 );

        newPlayerObject.transform.Find("Camera").gameObject.SetActive(true);
        ((MonoBehaviour)newPlayerObject.GetComponent("FPSController")).enabled = true;
    }
}
