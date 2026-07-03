using System.Collections.Generic;
using UnityEngine;

namespace GuildGame.Data
{
    /// <summary>학생 풀. 입장 학생 랜덤 선택 + 위조값(다른 학생의 값) 추출에 사용한다.</summary>
    [CreateAssetMenu(fileName = "StudentDatabase", menuName = "GuildGame/Student Database", order = 1)]
    public class StudentDatabaseSO : ScriptableObject
    {
        public List<StudentSO> students = new();

        public bool IsEmpty => students == null || students.Count == 0;
        public int Count => students?.Count ?? 0;

        public StudentSO GetRandom()
        {
            if (IsEmpty)
                return null;
            return students[Random.Range(0, students.Count)];
        }

        /// <summary><paramref name="exclude"/>와 다른 학생을 무작위로 반환(위조값 출처).</summary>
        public StudentSO GetRandomOther(StudentSO exclude)
        {
            if (IsEmpty)
                return null;
            if (students.Count == 1)
                return students[0] == exclude ? null : students[0];

            StudentSO pick;
            int guard = 0;
            do
            {
                pick = students[Random.Range(0, students.Count)];
            }
            while (pick == exclude && ++guard < 16);

            return pick == exclude ? null : pick;
        }
    }
}
