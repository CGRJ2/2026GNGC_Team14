using UnityEngine;

namespace MageAcademy.Gameplay.Models
{
    /// <summary>
    /// 수정구슬(알리바이) 검증 데이터(런타임). 학생의 주장 진술과, 수정구슬에 비친 어제 장면을 담는다.
    /// 진술이 장면과 모순되면(본인이 안 했음) 거짓이다.
    /// </summary>
    public class CrystalData
    {
        /// <summary>질문(플레이어가 던지는 물음) 로컬라이제이션 키.</summary>
        public string QuestionKey { get; }

        /// <summary>학생의 진술(주장) 로컬라이제이션 키.</summary>
        public string TestimonyKey { get; }

        /// <summary>수정구슬에 비친 어제 장면.</summary>
        public Sprite Scene { get; }

        /// <summary>진술이 장면과 모순되면(거짓말) true.</summary>
        public bool IsLie { get; }

        public CrystalData(string questionKey, string testimonyKey, Sprite scene, bool isLie)
        {
            QuestionKey = questionKey;
            TestimonyKey = testimonyKey;
            Scene = scene;
            IsLie = isLie;
        }

        public bool IsHonest => !IsLie;
    }
}
