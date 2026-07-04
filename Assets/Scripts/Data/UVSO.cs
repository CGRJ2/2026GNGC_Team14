using System.Collections.Generic;
using UnityEngine;

namespace MageAcademy.Data
{
    /// <summary>
    /// UV 지팡이 검증 config. 골렘이 만든 작품에는 흔적(서명)이 남는다.
    /// 흔적 스프라이트 후보군과 추궁 대사 키를 정의한다.
    /// </summary>
    [CreateAssetMenu(fileName = "UV", menuName = "MageAcademy/UV", order = 44)]
    public class UVSO : ScriptableObject
    {
        [Tooltip("골렘의 흔적(서명) 스프라이트 후보군. 골렘 작품일 때 랜덤으로 하나 사용")]
        public List<Sprite> signatureSprites = new();

        [Tooltip("흔적을 찾아 추궁할 때 플레이어 질문 키")]
        public string questionKey;

        [Tooltip("들킨 학생의 반응 대사 키(랜덤)")]
        public List<string> reactionKeys = new();

        public bool HasSignatures => signatureSprites != null && signatureSprites.Count > 0;

        public Sprite GetRandomSignature()
        {
            return HasSignatures ? signatureSprites[Random.Range(0, signatureSprites.Count)] : null;
        }

        public string GetRandomReactionKey()
        {
            return reactionKeys != null && reactionKeys.Count > 0
                ? reactionKeys[Random.Range(0, reactionKeys.Count)]
                : string.Empty;
        }
    }
}
