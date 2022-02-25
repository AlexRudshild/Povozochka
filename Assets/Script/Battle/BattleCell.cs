using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _image;

    private Color color = new Color(1, 1, 1);

    private BattleGrid _battleGrid = null;

    public Character Character = null;

    public Vector3 Position => transform.position;

    private void Start()
    {
        _image = transform.GetComponent<Image>();
    }

    public void SetBattleGrid(BattleGrid value)
    {
        _battleGrid = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _battleGrid.Select(this);
        }
    }

    public void UnSelect()
    {
        color = new Color(1, 1, 1, 1f);
        _image.color = color * new Color(1, 1, 1, 0.5f);
    }

    public void Select()
    {
        color = new Color(0, 1, 0, 1f);
        _image.color = color;
        // _image.color = new Color(0, 1, 0, 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = color * new Color(1, 1, 1, 0.5f);
    }
}
