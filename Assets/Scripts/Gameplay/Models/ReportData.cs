using System.Collections.Generic;
using MageAcademy.Data;

namespace MageAcademy.Gameplay.Models
{
    /// <summary>
    /// 학생이 제출한 레포트(런타임). 두 개의 위조 축을 가진다.
    /// - 헤더(이름·과목): 위조 시 다른 학생의 값이 표기됨 → 진술과 대조해 판별.
    /// - 본문: 여러 문단 중 하나가 외국어/특수 언어면 위조 → 시각 판별 후 해당 문단 클릭으로 추궁.
    /// 어떤 축도 위조되지 않았으면 정직하다.
    /// </summary>
    public class ReportData
    {
        private readonly List<string> _bodyKeys;

        /// <summary>레포트에 표기된 이름(위조 시 다른 학생 값).</summary>
        public string PrintedName { get; }

        /// <summary>레포트에 표기된 과목(위조 시 다른 학생 값).</summary>
        public string PrintedMajor { get; }

        public bool NameForged { get; }
        public bool MajorForged { get; }

        /// <summary>외국어로 위조된 문단 인덱스. 위조 없으면 -1.</summary>
        public int ForeignParagraphIndex { get; }

        public ReportData(
            string printedName, string printedMajor,
            IReadOnlyList<string> bodyKeys, int foreignParagraphIndex,
            bool nameForged, bool majorForged)
        {
            PrintedName = printedName;
            PrintedMajor = printedMajor;
            _bodyKeys = bodyKeys != null ? new List<string>(bodyKeys) : new List<string>();
            ForeignParagraphIndex = foreignParagraphIndex;
            NameForged = nameForged;
            MajorForged = majorForged;
        }

        /// <summary>본문 문단 개수.</summary>
        public int ParagraphCount => _bodyKeys.Count;

        /// <summary>문단 i의 로컬라이제이션 키.</summary>
        public string GetParagraphKey(int index)
        {
            return index >= 0 && index < _bodyKeys.Count ? _bodyKeys[index] : string.Empty;
        }

        /// <summary>문단 i가 외국어(위조)인지.</summary>
        public bool IsParagraphForeign(int index) => index == ForeignParagraphIndex;

        public bool BodyForged => ForeignParagraphIndex >= 0;

        /// <summary>어떤 축도 위조되지 않았으면 정직.</summary>
        public bool IsHonest => !NameForged && !MajorForged && !BodyForged;

        /// <summary>헤더 표기값(대조 대상 필드만 유효, 그 외 빈 문자열).</summary>
        public string GetPrinted(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return PrintedName;
                case StudentIdFieldType.Major: return PrintedMajor;
                default: return string.Empty;
            }
        }
    }
}
