using TMPro;
using UnityEngine;
using System.Collections;

public class NameText : MonoBehaviour
{
    private static NameText _instance;

    [SerializeField] private TMP_Text _text;

    private bool _showName = false;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public static void ShowName(string name)
    {
        if (name == string.Empty)
        {
            return;
        }

        _instance?.ShowNamePrivate(name);
    }

    public static void HideName()
    {
        _instance?.HideNamePrivate();
    }

	private IEnumerator DisplayName()
	{
		while (_showName)
		{
            _text.rectTransform.position = Input.mousePosition.Add(2.0f);
            yield return null;
		}
		yield break;
	}

    private void ShowNamePrivate(string name)
    {
        _text.enabled = true;
        _showName = _text.enabled;
        _text.text = name;
		StartCoroutine(DisplayName());
    }

    private void HideNamePrivate()
    {
        _text.enabled = false;
        _showName = _text.enabled;
		StopCoroutine(DisplayName());
    }
}
