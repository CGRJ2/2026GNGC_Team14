using System.Collections.Generic;
using UnityEngine;

namespace MageAcademy.Data
{
    /// <summary>
    /// 레포트 본문 문단 config. 본문은 여러 문단으로 구성되며, 각 문단 텍스트는 로컬라이제이션 키로 관리한다.
    /// - 표준 문단 키: 학교 표준 언어(정상 문단).
    /// - 외국어 문단 키: 다른 나라 언어/특수 룬(위조 문단). 위조 레포트는 한 문단만 여기서 채운다.
    /// 실제 텍스트는 Localization.csv에 작성하고, 여기엔 키 이름만 나열한다.
    /// </summary>
    [CreateAssetMenu(fileName = "Report", menuName = "MageAcademy/Report", order = 42)]
    public class ReportSO : ScriptableObject
    {
        [Min(1)]
        [Tooltip("한 레포트의 본문 문단 수")]
        public int paragraphCount = 4;

        [Tooltip("정상 문단 키(학교 표준 언어). 이 풀에서 문단 수만큼 뽑는다")]
        public List<string> paragraphStandardKeys = new();

        [Tooltip("위조 문단 키(외국어/특수 언어). 위조 시 한 문단을 이 풀에서 채운다")]
        public List<string> paragraphForeignKeys = new();

        public bool HasStandard => paragraphStandardKeys != null && paragraphStandardKeys.Count > 0;
        public bool HasForeign => paragraphForeignKeys != null && paragraphForeignKeys.Count > 0;

        public int ParagraphCount => Mathf.Max(1, paragraphCount);

        /// <summary>표준 문단 키를 중복 없이 count개 뽑는다(부족하면 중복 허용).</summary>
        public List<string> PickStandardKeys(int count)
        {
            var result = new List<string>(count);
            if (!HasStandard)
                return result;

            var pool = new List<string>(paragraphStandardKeys);
            for (int i = 0; i < count; i++)
            {
                if (pool.Count == 0)
                    pool.AddRange(paragraphStandardKeys); // 부족하면 재사용
                int idx = Random.Range(0, pool.Count);
                result.Add(pool[idx]);
                pool.RemoveAt(idx);
            }
            return result;
        }

        public string GetRandomForeignKey()
        {
            return HasForeign ? paragraphForeignKeys[Random.Range(0, paragraphForeignKeys.Count)] : string.Empty;
        }
    }
}
