using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>
    /// 플레이어가 던질 수 있는 질문 1개. 어떤 항목(<see cref="targetFact"/>)을 캐묻는지와
    /// 질문/답변 텍스트의 로컬라이제이션 키를 갖는다. 답변 템플릿의 {0}에 증언 값이 대입된다.
    /// </summary>
    [CreateAssetMenu(fileName = "Q_", menuName = "GuildGame/Question", order = 1)]
    public class QuestionSO : ScriptableObject
    {
        [Tooltip("고유 식별자")]
        public string questionId;

        [Tooltip("이 질문이 캐묻는 검증 항목")]
        public QuestFactType targetFact;

        [Tooltip("질문 텍스트 로컬라이제이션 키")]
        public string questionTextKey;

        [Tooltip("답변 템플릿 로컬라이제이션 키({0}에 값이 대입됨)")]
        public string answerTemplateKey;
    }
}
