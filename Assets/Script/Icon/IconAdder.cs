using System.Collections.Generic;
using UnityEngine;

public class IconAdder : MonoBehaviour
{
    private static IconAdder _instance;

    [SerializeField] private GameObject _iconPref;
    [SerializeField] private GameObject _contentMenu;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateIconsPrivate();
    }

    public static void UpdateIcons()
    {
        _instance?.UpdateIconsPrivate();
    }

    private void UpdateIconsPrivate()
    {
        var icons = _contentMenu.transform.GetComponentsInChildren<PreviewIcon>();

        foreach (var preview in icons)
        {
            DestroyImmediate(preview.gameObject);
        }

        var items = ItemManager.Instance.Items;

        foreach (var item in items)
        {
            var icon = Instantiate(_iconPref, _contentMenu.transform).GetComponent<PreviewIcon>();

            icon.Item = item;

            icon.UpdateSprite();

            icon.gameObject.name = item.Name;
            icon.RectTransform.localScale = Vector3.one;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
