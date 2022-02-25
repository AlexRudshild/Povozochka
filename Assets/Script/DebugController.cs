using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    [SerializeField] private GameObject FPScounter;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Debug FPS"))
        {
            FPScounter.SetActive(!FPScounter.activeSelf);
        }
    }
}
