using MageAcademy.Gameplay.Models;

namespace MageAcademy.Gameplay.Services
{
    /// <summary>랜덤 학생을 뽑고 위조(다른 학생 값 섞기)를 적용해 사건을 생성한다.</summary>
    public interface IStudentCaseGenerator
    {
        /// <param name="includeReport">이 날 레포트를 요구하면 레포트 검증면을 함께 생성한다.</param>
        StudentCase Generate(bool includeReport);
    }
}
