using System;
using GameKit.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GameKit.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaRect : MonoBehaviour
    {
        [Flags]
        private enum Direction
        {
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8
        }

        [SerializeField] private Direction direction;
        private RectTransform rt;

        private void Awake()
        {
            rt = (RectTransform) transform;
        }

        internal void Sync(Rect safeArea)
        {
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            if (direction.HasFlag(Direction.Left))
            {
                anchorMin.x /= Screen.width;
            }
            else
            {
                anchorMin.x = 0;
            }
            
            if (direction.HasFlag(Direction.Right))
            {
                anchorMax.x /= Screen.width;
            }
            else
            {
                anchorMax.x = 1;
            }

            if (direction.HasFlag(Direction.Top))
            {
                anchorMax.y /= Screen.height;
            }
            else
            {
                anchorMax.y = 1;
            }
            
            if (direction.HasFlag(Direction.Bottom))
            {
                anchorMin.y /= Screen.height;
            }
            else
            {
                anchorMin.y = 0;
            }
            
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            
            Debug.Log(safeArea);
        }
        
        private void OnEnable()
        {
            Service.Get<ProcessorSafeArea>().Subscribe(this);
        }

        private void OnDisable()
        {
            Service.Get<ProcessorSafeArea>().Unsubscribe(this);
        }

        private void Reset()
        {
            var rt = (RectTransform)transform;
            rt.anchorMax = new Vector2(1, 1);
            rt.anchorMin = new Vector2(0, 0);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);

            var canvas = GetComponent<Canvas>();
            if (canvas.isRootCanvas)
            {
                Debug.LogError($"{nameof(SafeAreaRect)} dont work with Root Canvas");
            }
        }
    }
}