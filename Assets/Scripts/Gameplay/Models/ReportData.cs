using System.Collections.Generic;

namespace MageAcademy.Gameplay.Models
{
    /// <summary>
    /// 학생이 제출한 레포트(런타임). 주제 하나와 그 주제의 본문 문단들로 구성된다.
    /// 위조 레포트는 한 문단만 다른 주제 또는 오류 그룹(깨진 언어) 텍스트로 교체된다.
    /// 어떤 문단도 교체되지 않았으면 정직하다. (헤더 이름·과목 대조는 폐지)
    /// </summary>
    public class ReportData
    {
        private readonly List<string> _bodyKeys;

        /// <summary>레포트 주제(제목) 로컬라이제이션 키.</summary>
        public string TopicKey { get; }

        /// <summary>위조된(다른 주제/오류) 문단 인덱스. 위조 없으면 -1.</summary>
        public int ForgedParagraphIndex { get; }

        public ReportData(string topicKey, IReadOnlyList<string> bodyKeys, int forgedParagraphIndex)
        {
            TopicKey = topicKey;
            _bodyKeys = bodyKeys != null ? new List<string>(bodyKeys) : new List<string>();
            ForgedParagraphIndex = forgedParagraphIndex;
        }

        /// <summary>본문 문단 개수.</summary>
        public int ParagraphCount => _bodyKeys.Count;

        /// <summary>문단 i의 로컬라이제이션 키.</summary>
        public string GetParagraphKey(int index)
        {
            return index >= 0 && index < _bodyKeys.Count ? _bodyKeys[index] : string.Empty;
        }

        /// <summary>문단 i가 위조(다른 주제/오류)인지.</summary>
        public bool IsParagraphForged(int index) => index == ForgedParagraphIndex;

        /// <summary>어떤 문단도 교체되지 않았으면 정직.</summary>
        public bool IsHonest => ForgedParagraphIndex < 0;
    }
}
