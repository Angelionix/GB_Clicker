using Unity.Mathematics;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    [SerializeField]private Camera _mainCamera;

    [SerializeField]private float _heightOnDragging;
    [SerializeField]private float _timeDelayBeforeDraggignStart;

    [SerializeField] private float _dragPorog;
    [SerializeField] private float timer;
    private Vector3 _mousePosOnNavPlane;
    private Vector3 _mouseStartDragPosition;
    private GameObject _dragTarget;
    [SerializeField]private bool _mouseIsOnObject = false;
    [SerializeField] private bool _isDragging = false;

    public  delegate void OnDragEnding();
    public static OnDragEnding onDragEnding;

    public delegate RaycastHit[] OnPress();
    public static OnPress onPress;

    private void Update()
    {
        if ((Input.GetMouseButtonUp(0)) && CheckObjectsUnderPointofClick())
        {
            if (_isDragging)
            {
                OnEndDrag(_dragTarget, _mousePosOnNavPlane);
                onDragEnding();
                _isDragging = false;
            }
            else
            {
                _dragTarget.GetComponent<EntityMakeMoney>().SpawnMoneyByClicking();
            }
            _mouseIsOnObject = false;

        }
        if ((Input.GetMouseButton(0)) && CheckObjectsUnderPointofClick())
        {
            Debug.Log($"{Input.touchCount}");
            if (Mathf.Abs((_mousePosOnNavPlane - _mouseStartDragPosition).magnitude) >= _dragPorog)
            {
                _isDragging = true;
                OnDraging(_dragTarget, _mousePosOnNavPlane);
            }         
        }
        if (Input.GetMouseButtonDown(0))
        {         
            _mouseStartDragPosition = _mousePosOnNavPlane;
        }
    }

    public bool CheckObjectsUnderPointofClick()
    {
        RaycastHit[] hits = onPress();
        if (hits.Length > 0)
        {
            for (int i =0; i<hits.Length; i++)
            {
                if (!_mouseIsOnObject)
                {
                    if (hits[i].collider.TryGetComponent<EntityData>(out EntityData entity) && !_isDragging)
                    {
                        _dragTarget = hits[i].collider.gameObject;
                        _mouseIsOnObject = true;
                    }
                    else
                    {
                        _mouseIsOnObject = false;
                    }
                }             
                if (hits[i].collider.TryGetComponent<Nav>(out Nav nav))
                {
                    _mousePosOnNavPlane = hits[i].point;
                }
            }
            return _mouseIsOnObject;
        }
        return false;
    }

    public void OnDraging(GameObject target, float3 newPos)
    {
        target.transform.position = Vector3.MoveTowards(target.transform.position, newPos, 0.5f);
    }
    public void OnEndDrag(GameObject target, float3 newPos)
    {
        target.transform.position = new float3(newPos.x, .5f, newPos.z);
    }
}
