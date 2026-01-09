using System;
using Helper.EventBusFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Fase03_Scripts.Sun
{
    public class Challenge03_Controller : MonoBehaviour
    {
        [SerializeField] private Slider sunSlider;
        [SerializeField] private Transform _sunSprite;

        [Header("Arc Sun Settings")] [SerializeField]
        private float horizontalRange = 500f;

        [SerializeField] private float arcHeight = 200f;
        [SerializeField] private Vector3 startPosition;
        [ContextMenu( "Initialize Slider" )]
        public void Initialize()
        {
            sunSlider.value = sunSlider.maxValue / 2;
            sunSlider.onValueChanged.AddListener(delegate { OnSliderChanged(); });
        }

        private void OnSliderChanged()
        {
            float t = sunSlider.value;
            float x = (t - 0.5f) * horizontalRange;
            float yOffset = 1 - Mathf.Pow((2 * t - 1), 2);
            float y = yOffset * arcHeight;
            _sunSprite.localPosition = new Vector3(x, y, 0) + startPosition;
            
            EventBus.Publish(new OnSliderChangeEvent(sunSlider.value));
        }
    }
}