using System;
using UnityEngine;

namespace UnityViperEngine {
    internal abstract class BaseInput : IDisposable {
        public event Action<Vector2> OnMousePositionMoved = default;
        public event Action<Vector2> OnLeftDown = default;
        public event Action OnLeftUp = default;
        public event Action OnRightDown = default;
        public event Action OnMiddleDown = default;

        public abstract void InputUpdate();
        public abstract void SetTolerance(float tolerance);
        public abstract void Dispose();
        public abstract Vector2 GetMousePosition();
        public abstract void InitializeMousePosition();

        protected void InvokeMousePositionMoved(Vector2 mousePosition) {
            OnMousePositionMoved?.Invoke(mousePosition);
        }
        protected void InvokeLeftDown() {
            OnLeftDown?.Invoke(GetMousePosition());
        }
        protected void InvokeLeftUp() {
            OnLeftUp?.Invoke();
        }
        protected void InvokeRightDown() {
            OnRightDown?.Invoke();
        }
        protected void InvokeMiddleDown() {
            OnMiddleDown?.Invoke();
        }
    }
}