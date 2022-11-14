using UnityEngine;

public class PointerCaster : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 _mousePosition;

    public delegate bool OnFindObjs(RaycastHit[] hits);
    public static OnFindObjs onFindObjs;

    private void OnEnable()
    {
        DragNDrop.onPress += CheckObjsUnderMouse;
    }

    private void OnDisable()
    {
        DragNDrop.onPress -= CheckObjsUnderMouse;
    }

    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    private RaycastHit[] CheckObjsUnderMouse()
    {
        _mousePosition = Input.mousePosition;
        return Physics.RaycastAll(_mainCamera.ScreenPointToRay(_mousePosition));
    }


}
