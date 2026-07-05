using System;
using TMPro;
using UnityEngine;

namespace MageAcademy.UI
{
    /// <summary>
    /// 인스펙터에서 TMP 텍스트의 폰트 에셋·크기·색을 설정하기 위한 재사용 스타일.
    /// 폰트 에셋이 비어 있으면 기존 폰트를 유지한다.
    /// </summary>
    [Serializable]
    public class TmpTextStyle
    {
        [Tooltip("폰트 에셋. 비우면 라벨의 기존 폰트를 유지")]
        public TMP_FontAsset font;

        [Min(1f)]
        [Tooltip("폰트 크기")]
        public float fontSize = 52f;

        [Tooltip("글자 색")]
        public Color color = Color.white;

        public void Apply(TMP_Text label)
        {
            if (label == null)
                return;

            if (font != null)
                label.font = font;
            label.fontSize = fontSize;
            label.color = color;
        }
    }
}
