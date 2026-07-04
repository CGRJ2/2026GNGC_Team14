using System.Collections.Generic;
using MageAcademy.Data;
using UnityEngine;

namespace MageAcademy.Gameplay.Models
{
    /// <summary>
    /// 현재 학생 1명의 사건(런타임). 진짜 학생(Student)과 학생증에 "표기된" 값(위조 반영)을 보유한다.
    /// 텍스트 필드의 카드 표기값과 얼굴 사진은 위조 시 다른 학생의 값이 들어간다.
    /// </summary>
    public class StudentCase : IVerifiableCase
    {
        private readonly Dictionary<StudentIdFieldType, bool> _forged;
        private readonly Dictionary<StudentIdFieldType, string> _cardText;

        /// <summary>진짜 본인(정답 기준).</summary>
        public StudentSO Student { get; }

        /// <summary>학생증에 표기된 얼굴 사진(위조 시 다른 학생 사진).</summary>
        public Sprite CardPhoto { get; }

        /// <summary>레포트를 요구하는 날의 제출 레포트. 없으면 null.</summary>
        public ReportData Report { get; }

        /// <summary>수정구슬을 요구하는 날의 알리바이 데이터. 없으면 null.</summary>
        public CrystalData Crystal { get; }

        /// <summary>UV 지팡이를 요구하는 날의 골렘 흔적 데이터. 없으면 null.</summary>
        public UVData UV { get; }

        public StudentCase(
            StudentSO student,
            Dictionary<StudentIdFieldType, bool> forged,
            Dictionary<StudentIdFieldType, string> cardText,
            Sprite cardPhoto,
            ReportData report = null,
            CrystalData crystal = null,
            UVData uv = null)
        {
            Student = student;
            _forged = forged ?? new Dictionary<StudentIdFieldType, bool>();
            _cardText = cardText ?? new Dictionary<StudentIdFieldType, string>();
            CardPhoto = cardPhoto;
            Report = report;
            Crystal = crystal;
            UV = uv;
        }

        /// <summary>학생증·레포트·수정구슬·UV 어떤 항목도 위조/거짓이 아니면 진짜(=정직).</summary>
        public bool IsHonest
        {
            get
            {
                foreach (var forged in _forged.Values)
                {
                    if (forged)
                        return false;
                }
                if (Report != null && !Report.IsHonest)
                    return false;
                if (Crystal != null && !Crystal.IsHonest)
                    return false;
                return UV == null || UV.IsHonest;
            }
        }

        public bool IsForged(StudentIdFieldType field)
        {
            return _forged.TryGetValue(field, out var f) && f;
        }

        /// <summary>학생증에 표기된 값(위조 시 다른 학생 값).</summary>
        public string GetCardText(StudentIdFieldType field)
        {
            return _cardText.TryGetValue(field, out var v) ? v : Student.GetText(field);
        }

        /// <summary>학생이 질문에 답할 때 말하는 진짜값.</summary>
        public string GetTrueText(StudentIdFieldType field)
        {
            return Student != null ? Student.GetText(field) : string.Empty;
        }

        /// <summary>실제 학생의 얼굴(정답 사진).</summary>
        public Sprite TruePhoto => Student != null ? Student.IdPhoto : null;
    }
}
