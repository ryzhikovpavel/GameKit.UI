using UnityEngine;
using UnityEngine.UI;

namespace GameKit.UI.Core
{
    public class ViewBlockable: ViewComponent
    {
        private GameObject block;

        public override bool Interactable
        {
            get { return block.activeSelf == false; }
            set
            {
                block.SetActive(value == false);
                if (value)
                    block.transform.SetAsFirstSibling();
                else
                    block.transform.SetAsLastSibling();
            }
        }

        internal override void Initialize()
        {
            base.Initialize();
            
            block = new GameObject("input block");
            block.transform.SetParent(transform);
            block.layer = gameObject.layer;
            var img = block.AddComponent<Image>();
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