using UnityEngine;
using UnityEngine.UI;

namespace GameKit.UI.Implementation
{
    public class ViewBlockable: ViewComponent
    {
        private GameObject _block;

        public override bool Interactable
        {
            get { return _block.activeSelf == false; }
            set
            {
                _block.SetActive(value == false);
                if (value)
                    _block.transform.SetAsFirstSibling();
                else
                    _block.transform.SetAsLastSibling();
            }
        }

        internal override void Initialize()
        {
            base.Initialize();
            
            _block = new GameObject("input block");
            _block.transform.SetParent(transform, false);
            _block.layer = gameObject.layer;
            var img = _block.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
            var rt = img.rectTransform;
            rt.anchorMax = new Vector2(1, 1);
            rt.anchorMin = new Vector2(0, 0);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);
            rt.SetAsFirstSibling();
            img.SetActive(false);
        }
    }
}