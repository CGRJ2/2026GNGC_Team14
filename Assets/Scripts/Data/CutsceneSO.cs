using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    [CreateAssetMenu(fileName = "Cutscene", menuName = "GuildGame/Cutscene", order = 30)]
    public class CutsceneSO : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private List<Line> _lines = new();

        public string Id => _id;
        public IReadOnlyList<Line> Lines => _lines;

        [Serializable]
        public class Line
        {
            public CutsceneSpeaker speaker = CutsceneSpeaker.Student;
            public string localizationKey;
            public bool useRandomVariant = true;

            [Tooltip("Negative values use UIAnimationSettings.cutsceneLineDelay.")]
            public float delayAfter = -1f;
        }
    }
}
