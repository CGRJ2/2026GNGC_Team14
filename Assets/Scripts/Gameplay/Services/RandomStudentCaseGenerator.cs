using System.Collections.Generic;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using UnityEngine;

namespace MageAcademy.Gameplay.Services
{
    /// <summary>
    /// 랜덤 학생 선택 후 확률적으로 1~2개 필드를 위조한다. 위조값은 DB의 다른 학생 값을 섞어 만든다.
    /// (텍스트: 다른 학생의 같은 필드값, 얼굴: 다른 학생의 사진)
    /// </summary>
    public class RandomStudentCaseGenerator : IStudentCaseGenerator
    {
        private static readonly StudentIdFieldType[] AllFields =
        {
            StudentIdFieldType.Name,
            StudentIdFieldType.EnrollmentDate,
            StudentIdFieldType.Grade,
            StudentIdFieldType.Major,
            StudentIdFieldType.FacePhoto
        };

        private readonly StudentDatabaseSO _database;
        private readonly float _forgeChance;

        public RandomStudentCaseGenerator(StudentDatabaseSO database, float forgeChance)
        {
            _database = database;
            _forgeChance = Mathf.Clamp01(forgeChance);
        }

        public StudentCase Generate()
        {
            StudentSO real = _database != null ? _database.GetRandom() : null;
            if (real == null)
                return null;

            var forged = new Dictionary<StudentIdFieldType, bool>();
            var cardText = new Dictionary<StudentIdFieldType, string>();
            foreach (var field in AllFields)
            {
                forged[field] = false;
                if (field != StudentIdFieldType.FacePhoto)
                    cardText[field] = real.GetText(field);
            }
            Sprite cardPhoto = real.IdPhoto;

            if (Random.value < _forgeChance)
            {
                var order = new List<StudentIdFieldType>(AllFields);
                Shuffle(order);

                int target = Random.Range(1, 3); // 1~2개 위조 시도
                int applied = 0;
                foreach (var field in order)
                {
                    if (applied >= target)
                        break;
                    if (TryForge(field, real, forged, cardText, ref cardPhoto))
                        applied++;
                }
            }

            return new StudentCase(real, forged, cardText, cardPhoto);
        }

        private bool TryForge(
            StudentIdFieldType field, StudentSO real,
            Dictionary<StudentIdFieldType, bool> forged,
            Dictionary<StudentIdFieldType, string> cardText,
            ref Sprite cardPhoto)
        {
            if (field == StudentIdFieldType.FacePhoto)
            {
                for (int i = 0; i < 8; i++)
                {
                    StudentSO other = _database.GetRandomOther(real);
                    if (other == null)
                        return false;
                    if (other.IdPhoto != null && other.IdPhoto != real.IdPhoto)
                    {
                        cardPhoto = other.IdPhoto;
                        forged[field] = true;
                        return true;
                    }
                }
                return false;
            }

            string trueValue = real.GetText(field);
            for (int i = 0; i < 8; i++)
            {
                StudentSO other = _database.GetRandomOther(real);
                if (other == null)
                    return false;
                string otherValue = other.GetText(field);
                if (!string.IsNullOrEmpty(otherValue) && otherValue != trueValue)
                {
                    cardText[field] = otherValue;
                    forged[field] = true;
                    return true;
                }
            }
            return false;
        }

        private static void Shuffle(IList<StudentIdFieldType> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
