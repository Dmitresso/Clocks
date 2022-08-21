#region USING

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#endregion



namespace NRTW.Code.Scripts.Arrows {
    public class DragAndRotate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
        #region FIELDS
        [SerializeField] protected bool draggable = true;
        
        
        protected UnityEvent<PointerEventData>[] dragEvents;
        protected UnityEvent<PointerEventData> OnBeginDragEvent = new ();
        protected UnityEvent<PointerEventData> OnDragEvent = new ();
        protected UnityEvent<PointerEventData> OnDropEvent = new ();
        protected UnityEvent<PointerEventData> OnEndDragEvent = new ();
        #endregion



        #region BUILTIN METHODS
        protected void Awake() {
            dragEvents = new [] { OnBeginDragEvent, OnDragEvent, OnEndDragEvent, OnDropEvent };
        }


        protected void OnDisable() {
            StopAllCoroutines();
            foreach (var @event in dragEvents) @event.RemoveAllListeners();
        }


        public void OnBeginDrag(PointerEventData eventData) {
            if (draggable) OnBeginDragEvent?.Invoke(eventData);
        }

        
        public void OnDrag(PointerEventData eventData) {
            if (draggable) OnDragEvent?.Invoke(eventData);
        }


        public void OnEndDrag(PointerEventData eventData) {
            OnEndDragEvent?.Invoke(eventData);
        }

        
        public void OnDrop(PointerEventData eventData) {
            OnDropEvent?.Invoke(eventData);
        }
        #endregion
        
        
        
        #region CUSTOM METHODS
        protected void SwitchBehavior(UnityEvent<PointerEventData> @event, UnityAction<PointerEventData> behavior) {
            @event.RemoveAllListeners();
            @event.AddListener(behavior);
        }
        #endregion
    }
}