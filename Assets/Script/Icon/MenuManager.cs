using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private IconAdder _iconAdder;

    private void Update()
    {

        if (Input.GetButtonDown("AddMenu"))
        {
            _iconAdder.gameObject.SetActive(!_iconAdder.isActiveAndEnabled);
            if (_iconAdder.isActiveAndEnabled)
            {
                IconAdder.UpdateIcons();
            }
        }
    }
}
