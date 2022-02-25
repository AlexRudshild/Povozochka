using UnityEngine;
using UnityEngine.EventSystems;

public class MoveWindow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private bool _destroyParent = false;

	private Transform _window;
	private Rect _bound;
	private Rect _trueBound;

	private Vector3 _holdPosition;

	private bool _dragWindow;

	private void Awake()
	{
		_window = transform.parent.transform;
	}

	void Update()
	{
		if (_dragWindow)
		{
			_window.position = TrueWindowPos(Input.mousePosition);
		}
	}

	private Vector2 TrueWindowPos(Vector2 mousePositon)
	{
		float x, y;

		if (mousePositon.x >= _bound.width)
		{
			x = _trueBound.width;
		}
		else if (mousePositon.x <= _bound.x)
		{
			x = _trueBound.x;
		}
		else
		{
			x = mousePositon.x - _holdPosition.x;
		}

		if (mousePositon.y >= _bound.height)
		{
			y = _trueBound.height;
		}
		else if (mousePositon.y <= _bound.y)
		{
			y = _trueBound.y;
		}
		else
		{
			y = mousePositon.y - _holdPosition.y;
		}

		return new Vector2(x, y);
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (data.button == PointerEventData.InputButton.Left)
		{
			_holdPosition = Input.mousePosition - _window.position;
			var rect = transform.parent.GetComponent<RectTransform>().rect;
			var releation = (Screen.width / GlobalParametrs.ScreenSize.x + Screen.height / GlobalParametrs.ScreenSize.y) / 2;

			_trueBound = new Rect(rect.width / 2 * releation, rect.height / 2 * releation, Screen.width - rect.width / 2 * releation, Screen.height - rect.height / 2 * releation);

			_bound = new Rect(_holdPosition.x + _trueBound.x, 
				_holdPosition.y + _trueBound.y, 
				_holdPosition.x + _trueBound.width,
				_holdPosition.y + _trueBound.height);


			transform.parent.SetAsLastSibling();

			InventoryControl.Instance.ReFillListInvetors();

			_dragWindow = true;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (data.button == PointerEventData.InputButton.Left)
		{
			_holdPosition = Vector3.zero;
			_dragWindow = false;
		}
	}

	public void Close()
	{
		if (_destroyParent)
			DestroyImmediate(transform.parent.gameObject);
		else
			transform.parent.gameObject.SetActive(false);
	}
}
