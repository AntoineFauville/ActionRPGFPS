using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingCamera : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Screen.lockCursor = false;
        }
        else
            Screen.lockCursor = true;
    }
}
