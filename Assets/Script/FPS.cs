using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(FPScounter))]

public class FPS : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    [SerializeField] private FPScounter _counter;

    private string[] _intToString;

    // Start is called before the first frame update
    void Awake()
    {
        _intToString = new string[500];

        for (int i = 0; i < _intToString.Length; i++)
        {
            _intToString[i] = i.ToString();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _text.text = _intToString[Mathf.Clamp(_counter.AverageFPS, 0, 499)];
    }
}
