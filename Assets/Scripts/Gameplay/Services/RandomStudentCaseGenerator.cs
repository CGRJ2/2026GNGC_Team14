using System.Collections.Generic;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using UnityEngine;

namespace MageAcademy.Gameplay.Services
{
    /// <summary>
    /// 랜덤 학생을 뽑고 위조를 적용한다.
    /// - 레포트 미포함 날: 기존 방식(정직/위조 판정 후 학생증 1~2개 위조).
    /// - 레포트 포함 날: 정직/위조 판정 후, 위조면 {학생증필드 / 레포트헤더 / 레포트본문} 중 한 축만 위조.
    /// 위조값은 DB의 다른 학생 값을 섞어 만든다.
    /// </summary>
    public class RandomStudentCaseGenerator : IStudentCaseGenerator
    {
        private enum ForgeAxis { IdField, ReportHeader, ReportBody }

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
        private readonly ReportSO _report;

        public RandomStudentCaseGenerator(StudentDatabaseSO database, float forgeChance, ReportSO report = null)
        {
            _database = database;
            _forgeChance = Mathf.Clamp01(forgeChance);
            _report = report;
        }

        public StudentCase Generate(bool includeReport)
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

            // 정직 레포트를 먼저 만든다(포함 날에 한해).
            ReportData report = includeReport ? BuildHonestReport(real) : null;

            if (Random.value < _forgeChance)
            {
                if (includeReport)
                    report = ForgeSingleTell(real, forged, cardText, ref cardPhoto, report);
                else
                    ForgeIdFields(real, forged, cardText, ref cardPhoto, Random.Range(1, 3));
            }

            return new StudentCase(real, forged, cardText, cardPhoto, report);
        }

        private ReportData BuildHonestReport(StudentSO real)
        {
            int count = _report != null ? _report.ParagraphCount : 1;
            List<string> keys = _report != null ? _report.PickStandardKeys(count) : new List<string>();
            return new ReportData(
                printedName: real.studentName,
                printedMajor: real.major,
                bodyKeys: keys,
                foreignParagraphIndex: -1,
                nameForged: false,
                majorForged: false);
        }

        private static List<string> ExtractKeys(ReportData report)
        {
            var keys = new List<string>(report.ParagraphCount);
            for (int i = 0; i < report.ParagraphCount; i++)
                keys.Add(report.GetParagraphKey(i));
            return keys;
        }

        /// <summary>세 축 중 실현 가능한 한 곳만 위조한다. 실패 시 다른 축으로 폴백.</summary>
        private ReportData ForgeSingleTell(
            StudentSO real,
            Dictionary<StudentIdFieldType, bool> forged,
            Dictionary<StudentIdFieldType, string> cardText,
            ref Sprite cardPhoto,
            ReportData honestReport)
        {
            var axes = new List<ForgeAxis> { ForgeAxis.IdField, ForgeAxis.ReportHeader, ForgeAxis.ReportBody };
            Shuffle(axes);

            foreach (var axis in axes)
            {
                switch (axis)
                {
                    case ForgeAxis.IdField:
                        if (ForgeIdFields(real, forged, cardText, ref cardPhoto, 1) > 0)
                            return honestReport;
                        break;

                    case ForgeAxis.ReportHeader:
                        ReportData headerForged = ForgeReportHeader(real, honestReport);
                        if (headerForged != null)
                            return headerForged;
                        break;

                    case ForgeAxis.ReportBody:
                        if (_report != null && _report.HasForeign && honestReport.ParagraphCount > 0)
                        {
                            var keys = ExtractKeys(honestReport);
                            int idx = Random.Range(0, keys.Count);
                            keys[idx] = _report.GetRandomForeignKey();
                            return new ReportData(
                                honestReport.PrintedName, honestReport.PrintedMajor,
                                keys, foreignParagraphIndex: idx,
                                nameForged: false, majorForged: false);
                        }
                        break;
                }
            }

            // 어떤 축도 위조 못 하면 정직 유지(희귀).
            return honestReport;
        }

        /// <summary>레포트 헤더(이름 또는 과목)를 다른 학생 값으로 위조한다. 실패 시 null.</summary>
        private ReportData ForgeReportHeader(StudentSO real, ReportData honest)
        {
            bool forgeName = Random.value < 0.5f;

            for (int attempt = 0; attempt < 2; attempt++)
            {
                bool tryName = forgeName ^ (attempt == 1);
                for (int i = 0; i < 8; i++)
                {
                    StudentSO other = _database.GetRandomOther(real);
                    if (other == null)
                        return null;

                    if (tryName)
                    {
                        if (!string.IsNullOrEmpty(other.studentName) && other.studentName != real.studentName)
                            return new ReportData(other.studentName, honest.PrintedMajor, ExtractKeys(honest),
                                foreignParagraphIndex: -1, nameForged: true, majorForged: false);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(other.major) && other.major != real.major)
                            return new ReportData(honest.PrintedName, other.major, ExtractKeys(honest),
                                foreignParagraphIndex: -1, nameForged: false, majorForged: true);
                    }
                }
            }
            return null;
        }

        /// <summary>학생증 필드를 최대 target개 위조한다. 실제 위조된 개수를 반환.</summary>
        private int ForgeIdFields(
            StudentSO real,
            Dictionary<StudentIdFieldType, bool> forged,
            Dictionary<StudentIdFieldType, string> cardText,
            ref Sprite cardPhoto,
            int target)
        {
            var order = new List<StudentIdFieldType>(AllFields);
            Shuffle(order);

            int applied = 0;
            foreach (var field in order)
            {
                if (applied >= target)
                    break;
                if (TryForge(field, real, forged, cardText, ref cardPhoto))
                    applied++;
            }
            return applied;
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

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
