using GuildGame.Gameplay.Models;

namespace GuildGame.Gameplay.Services
{
    /// <summary>랜덤 학생을 뽑고 위조(다른 학생 값 섞기)를 적용해 사건을 생성한다.</summary>
    public interface IStudentCaseGenerator
    {
        StudentCase Generate();
    }
}
