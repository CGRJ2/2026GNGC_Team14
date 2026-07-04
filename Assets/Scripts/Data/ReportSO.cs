using System;
using System.Collections.Generic;
using UnityEngine;

namespace MageAcademy.Data
{
    /// <summary>
    /// 레포트 본문 그룹 config. 각 그룹 = 하나의 주제 + 그 주제의 본문 문단들.
    /// 정상 레포트는 한 그룹의 문단으로만 구성되고, 위조 레포트는 한 문단만
    /// 다른 그룹(주제) 또는 오류 그룹(깨진 언어)의 텍스트로 교체된다.
    /// 실제 텍스트는 Localization.csv에 작성하고, 여기엔 키 이름만 나열한다.
    /// </summary>
    [CreateAssetMenu(fileName = "Report", menuName = "MageAcademy/Report", order = 42)]
    public class ReportSO : ScriptableObject
    {
        [Serializable]
        public class ReportGroup
        {
            [Tooltip("주제 제목 로컬라이제이션 키")]
            public string topicKey;

            [Tooltip("이 주제의 본문 문단 키들(순서대로)")]
            public List<string> paragraphKeys = new();

            public bool HasEnough(int count) => paragraphKeys != null && paragraphKeys.Count >= count;

            public string GetRandomParagraph()
            {
                return paragraphKeys != null && paragraphKeys.Count > 0
                    ? paragraphKeys[UnityEngine.Random.Range(0, paragraphKeys.Count)]
                    : string.Empty;
            }
        }

        [Min(1)]
        [Tooltip("한 레포트의 본문 문단 수")]
        public int paragraphCount = 4;

        [Tooltip("주제 그룹들. 각 그룹은 주제 + 문단 4개 이상")]
        public List<ReportGroup> groups = new();

        [Tooltip("오류 그룹: 깨진 언어/외국어 본문 키(위조 문단 후보)")]
        public List<string> errorKeys = new();

        public int ParagraphCount => Mathf.Max(1, paragraphCount);
        public bool HasGroups => groups != null && groups.Count > 0;
        public bool HasError => errorKeys != null && errorKeys.Count > 0;

        public ReportGroup GetRandomGroup()
        {
            return HasGroups ? groups[UnityEngine.Random.Range(0, groups.Count)] : null;
        }

        /// <summary>주어진 그룹과 다른 그룹을 반환한다. 그룹이 하나뿐이면 null.</summary>
        public ReportGroup GetRandomOtherGroup(ReportGroup group)
        {
            if (!HasGroups || groups.Count < 2)
                return null;

            for (int i = 0; i < 16; i++)
            {
                ReportGroup candidate = groups[UnityEngine.Random.Range(0, groups.Count)];
                if (candidate != group)
                    return candidate;
            }
            return null;
        }

        public string GetRandomErrorKey()
        {
            return HasError ? errorKeys[UnityEngine.Random.Range(0, errorKeys.Count)] : string.Empty;
        }
    }
}
