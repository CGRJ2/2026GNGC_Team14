using System;
using UnityEngine;

namespace MageAcademy.Data
{
    [CreateAssetMenu(fileName = "EndingSettings", menuName = "MageAcademy/Ending Settings", order = 42)]
    public class EndingSettingsSO : ScriptableObject
    {
        [Serializable]
        public class EndingPresentation
        {
            public GameObject eventPrefab;

            public Sprite illustration;

            [TextArea]
            public string text;

            [Min(0f)]
            public float duration = 5f;
        }

        [Header("Reputation Thresholds")]
        public int badMaxReputation = 8;
        public int normalMaxReputation = 15;

        [Header("Scene")]
        public string titleSceneName = "TitleScene";

        [Header("Ending Presentations")]
        public EndingPresentation badEnding = new()
        {
            text = "Bad Ending",
            duration = 5f,
        };

        public EndingPresentation normalEnding = new()
        {
            text = "Normal Ending",
            duration = 5f,
        };

        public EndingPresentation goodEnding = new()
        {
            text = "Good Ending",
            duration = 5f,
        };

        public EndingType GetEndingType(int reputation)
        {
            if (reputation <= badMaxReputation)
                return EndingType.Bad;
            if (reputation <= normalMaxReputation)
                return EndingType.Normal;
            return EndingType.Good;
        }

        public EndingPresentation GetPresentation(int reputation)
        {
            return GetEndingType(reputation) switch
            {
                EndingType.Bad => badEnding,
                EndingType.Normal => normalEnding,
                EndingType.Good => goodEnding,
                _ => normalEnding,
            };
        }
    }
}
