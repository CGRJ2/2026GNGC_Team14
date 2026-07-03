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
            public CutsceneStepType stepType = CutsceneStepType.Dialogue;
            public CutsceneSpeaker speaker = CutsceneSpeaker.Student;

            [Tooltip("StudentSO.studentId. Used when speaker is Student.")]
            public string studentId;

            public string localizationKey;

            [Tooltip("Negative values use UIAnimationSettings.cutsceneLineDelay.")]
            public float delayAfter = -1f;
        }
    }
}
