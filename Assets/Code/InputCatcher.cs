using Assets.Code.BoardHelpers;
using Assets.Code.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class InputCatcher : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private static InputCatcher _instance;

        public static InputCatcher Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<InputCatcher>();

                return _instance;
            }
        }

        private OnMove _onMove;

        public OnMove OnMove => _onMove;

        public bool IsLocked { get; set; }

        private Vector2 _beginDragPosition;

        private void Awake()
        {
            _onMove = new OnMove();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _beginDragPosition = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var delta = eventData.position - _beginDragPosition;

            if (!IsLocked && delta.magnitude > 0f)
                OnMove.Invoke(BoardMovementHelper.DirectionToMovementDirection(delta));
        }

        public void OnDrag(PointerEventData eventData)
        {
            //pass
        }
    }
}
