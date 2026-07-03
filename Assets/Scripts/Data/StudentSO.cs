using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>
    /// 학생 1명 = 단일 일러스트 + 진짜 신원 데이터. 값은 SO에 직접 문자열로 저장한다.
    /// 위조는 런타임에 다른 학생의 값을 섞어 만든다(여기엔 진짜값만 존재).
    /// </summary>
    [CreateAssetMenu(fileName = "Student", menuName = "GuildGame/Student", order = 0)]
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

        public Sprite IdPhoto => idPhoto != null ? idPhoto : illustration;

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
