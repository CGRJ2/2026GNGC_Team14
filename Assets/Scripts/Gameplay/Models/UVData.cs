using UnityEngine;

namespace MageAcademy.Gameplay.Models
{
    /// <summary>
    /// UV 지팡이 검증 데이터(런타임). 골렘이 만든 작품이면 흔적(서명)이 남는다.
    /// 흔적이 있으면(골렘 작품) 거짓이며, 플레이어가 UV로 흔적을 찾아 추궁한다.
    /// </summary>
    public class UVData
    {
        /// <summary>골렘이 만든 작품이면 true(흔적 존재).</summary>
        public bool IsGolemWork { get; }

        /// <summary>골렘의 흔적(서명) 스프라이트. 골렘 작품이 아니면 null.</summary>
        public Sprite SignatureSprite { get; }

        /// <summary>흔적을 추궁할 때 플레이어 질문 키.</summary>
        public string QuestionKey { get; }

        /// <summary>들킨 학생의 반응 대사 키.</summary>
        public string ReactionKey { get; }

        public UVData(bool isGolemWork, Sprite signatureSprite, string questionKey, string reactionKey)
        {
            IsGolemWork = isGolemWork;
            SignatureSprite = signatureSprite;
            QuestionKey = questionKey;
            ReactionKey = reactionKey;
        }

        public bool IsHonest => !IsGolemWork;
    }
}
