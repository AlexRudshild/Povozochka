using UnityEngine;

public class BattleGrid : MonoBehaviour
{
    [SerializeField] private BattleCell[,] _cells = null;
    [SerializeField] private GameObject _cellPrefab = null;

    [Header("Rect")]
    
    [SerializeField] private RectTransform _gridRect = null;


    [Header("Grid Settings")]

    [SerializeField] private int _height = 0;
    [SerializeField] private int _width = 0;
    [SerializeField] private Character[] characters = null;

    private BattleCell _selectedCell = null;
    private Character _selectedCharacter = null;

    void Start()
    {
        BuildGrid();
        characters[0].MoveTo(_cells[0, 0].Position);
        _cells[0, 0].Character = characters[0];

        characters[1].MoveTo(_cells[0, 10].Position);
        _cells[0, 10].Character = characters[1];

        characters[0].SetStats(10, 1);
        characters[1].SetStats(10, 2);
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.positiveInfinity, 1);
        if (hit)
        {
            Debug.Log(hit.collider.name);
        }
    }

    public void Select(BattleCell cell)
    {
        if (cell.Character != null && _selectedCharacter != null)
        {
            cell.Character.MinusHp(_selectedCharacter.Damage);
            return;
        }

        _selectedCell?.UnSelect();

        if (cell.Character != null)
        {

            cell.Select();

            _selectedCharacter = cell.Character;

            _selectedCell = cell;

            return;
        }

        if (_selectedCharacter == null)
        {
            return;
        }

        _selectedCharacter.MoveTo(cell.Position);
        cell.Character = _selectedCharacter;

        _selectedCharacter = null;
        _selectedCell.Character = null;

        _selectedCell = cell;
    }

    public void BuildGrid()
    {
        var cells = this.transform.GetComponentsInChildren<BattleCell>();

        foreach (var cell in cells)
        {
            DestroyImmediate(cell.gameObject);
        }

        var cellSize = GlobalParametrs.ScreenSize.y / _height - 5;
        //-43 70
        Vector2 delta = new Vector2(cellSize * _width * 1.15f + cellSize / 2, cellSize * _height * 0.86f);

        //_rectTransform.sizeDelta = new Vector2(delta.x + 1, delta.y + 1 + _toolbarRect.sizeDelta.y);
        //_gridRect.anchoredPosition = new Vector2(0, -_toolbarRect.sizeDelta.y);
        _gridRect.sizeDelta = delta;

        // var test = new Material(_shaderGrid);

        // test.SetVector("GridSize", new Vector2(_width, _height));

        // _gridRect.gameObject.GetComponent<Image>().material = test;

        float posX = -(cellSize * _width / 2f + cellSize / 2f) * 1.15f;
        float posY = (cellSize * _height / 2f + cellSize / 2f) * 0.86f;
        float Xreset = posX;
        int i = 0;
        _cells = new BattleCell[_height, _width];

        var q = true;

        for (int y = 0; y < _height; y++)
        {
            posY -= cellSize * 0.86f;
            for (int x = 0; x < _width; x++)
            {
                posX += cellSize * 1.15f;
                RectTransform tr = Instantiate(_cellPrefab, _gridRect).GetComponent<RectTransform>();
                tr.sizeDelta = new Vector2(cellSize * 1.15f, cellSize * 1.15f);
                tr.localScale = Vector3.one;
                tr.anchoredPosition = new Vector2(posX, posY);
                tr.name = "Cell_" + i;
                // tr.GetComponent<InventoryCells>().InventoryId = Id;
                _cells[y, x] = tr.GetComponent<BattleCell>();
                _cells[y, x].SetBattleGrid(this);
                i++;
            }
            q = q ? false : true;
            posX = q ? Xreset : Xreset - cellSize / 2 * 1.15f;
        }
        // _nameLabel.text = name;
    }
}
