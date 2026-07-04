using UnityEngine;

namespace MageAcademy.Data
{
    /// <summary>
    /// 학생 1명 = 단일 일러스트 + 진짜 신원 데이터. 값은 SO에 직접 문자열로 저장한다.
    /// 위조는 런타임에 다른 학생의 값을 섞어 만든다(여기엔 진짜값만 존재).
    /// </summary>
    [CreateAssetMenu(fileName = "Student", menuName = "MageAcademy/Student", order = 0)]
    public class StudentSO : ScriptableObject
    {
        [Header("Identity (진짜값)")]
        public string studentId;
        public string studentName;
        public string enrollmentDate;
        public string grade;
        public string major;

        [Header("Visuals")]
        [Tooltip("테이블/HUD에 표시되는 단일 일러스트")]
        public Sprite illustration;

        [Tooltip("학생증 얼굴 사진(미지정 시 illustration 사용)")]
        public Sprite idPhoto;

        [Header("표정 일러스트 (미지정 시 기본 illustration 폴백)")]
        [Tooltip("기쁨: 참인데 통과시켰을 때")]
        public Sprite happyIllustration;

        [Tooltip("당황: 거짓 정보를 추궁당했을 때")]
        public Sprite flusteredIllustration;

        [Tooltip("화남: 검사 제한시간을 초과했을 때")]
        public Sprite angryIllustration;

        [Tooltip("비열: 거짓인데 통과시켰을 때")]
        public Sprite sneerIllustration;

        public Sprite IdPhoto => idPhoto != null ? idPhoto : illustration;

        /// <summary>표정에 맞는 일러스트를 반환한다(미지정 시 기본 illustration).</summary>
        public Sprite GetEmotionSprite(StudentEmotion emotion)
        {
            Sprite sprite;
            switch (emotion)
            {
                case StudentEmotion.Happy: sprite = happyIllustration; break;
                case StudentEmotion.Flustered: sprite = flusteredIllustration; break;
                case StudentEmotion.Angry: sprite = angryIllustration; break;
                case StudentEmotion.Sneer: sprite = sneerIllustration; break;
                default: sprite = illustration; break;
            }
            return sprite != null ? sprite : illustration;
        }

        /// <summary>텍스트 필드의 진짜값을 반환한다(FacePhoto는 빈 문자열).</summary>
        public string GetText(StudentIdFieldType field)
        {
            switch (field)
            {
                case StudentIdFieldType.Name: return studentName;
                case StudentIdFieldType.EnrollmentDate: return enrollmentDate;
                case StudentIdFieldType.Grade: return grade;
                case StudentIdFieldType.Major: return major;
                default: return string.Empty;
            }
        }
    }
}
