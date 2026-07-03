using System;

namespace GuildGame.Data
{
    /// <summary>
    /// 의뢰의 검증 항목 하나. 참값과 (디자이너가 authoring한) 그럴듯한 거짓값을 로컬라이제이션
    /// 키 쌍으로 보유한다. 증언 생성 시 거짓으로 지정되면 <see cref="lieValueKey"/>가 사용된다.
    /// </summary>
    [Serializable]
    public class QuestFact
    {
        public QuestFactType type;

        [UnityEngine.Tooltip("참값 로컬라이제이션 키")]
        public string trueValueKey;

        [UnityEngine.Tooltip("거짓일 때 표시할 값의 로컬라이제이션 키")]
        public string lieValueKey;
    }
}
