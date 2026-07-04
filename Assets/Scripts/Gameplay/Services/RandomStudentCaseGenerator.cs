using System.Collections.Generic;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using UnityEngine;

namespace MageAcademy.Gameplay.Services
{
    /// <summary>
    /// 랜덤 학생을 뽑고 위조를 적용한다.
    /// - 레포트 미포함 날: 정직/위조 판정 후 학생증 1~2개 위조.
    /// - 레포트 포함 날: 정직/위조 판정 후, 위조면 {학생증필드 / 레포트본문} 중 한 축만 위조.
    ///   레포트 본문 위조는 한 그룹(주제)의 문단 하나를 다른 주제 또는 오류 그룹 텍스트로 교체한다.
    /// </summary>
    public class RandomStudentCaseGenerator : IStudentCaseGenerator
    {
        private enum ForgeAxis { IdField, ReportBody }

        private static readonly StudentIdFieldType[] AllFields =
        {
            StudentIdFieldType.Name,
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

            // 정직 레포트를 먼저 만든다(포함 날 & 그룹 데이터 존재 시).
            ReportSO.ReportGroup group = null;
            ReportData report = null;
            if (includeReport && _report != null && _report.HasGroups)
            {
                group = _report.GetRandomGroup();
                report = BuildHonestReport(group);
            }

            if (Random.value < _forgeChance)
            {
                if (report != null)
                    report = ForgeSingleTell(real, forged, cardText, ref cardPhoto, group, report);
                else
                    ForgeIdFields(real, forged, cardText, ref cardPhoto, Random.Range(1, 3));
            }

            return new StudentCase(real, forged, cardText, cardPhoto, report);
        }

        private ReportData BuildHonestReport(ReportSO.ReportGroup group)
        {
            int count = _report.ParagraphCount;
            var keys = new List<string>(count);
            for (int i = 0; i < count && i < group.paragraphKeys.Count; i++)
                keys.Add(group.paragraphKeys[i]);
            return new ReportData(group.topicKey, keys, forgedParagraphIndex: -1);
        }

        private static List<string> ExtractKeys(ReportData report)
        {
            var keys = new List<string>(report.ParagraphCount);
            for (int i = 0; i < report.ParagraphCount; i++)
                keys.Add(report.GetParagraphKey(i));
            return keys;
        }

        /// <summary>실현 가능한 한 축만 위조한다. 실패 시 다른 축으로 폴백.</summary>
        private ReportData ForgeSingleTell(
            StudentSO real,
            Dictionary<StudentIdFieldType, bool> forged,
            Dictionary<StudentIdFieldType, string> cardText,
            ref Sprite cardPhoto,
            ReportSO.ReportGroup group,
            ReportData honestReport)
        {
            var axes = new List<ForgeAxis> { ForgeAxis.IdField, ForgeAxis.ReportBody };
            Shuffle(axes);

            foreach (var axis in axes)
            {
                if (axis == ForgeAxis.IdField)
                {
                    if (ForgeIdFields(real, forged, cardText, ref cardPhoto, 1) > 0)
                        return honestReport;
                }
                else
                {
                    ReportData bodyForged = ForgeReportBody(group, honestReport);
                    if (bodyForged != null)
                        return bodyForged;
                }
            }

            // 어떤 축도 위조 못 하면 정직 유지(희귀).
            return honestReport;
        }

        /// <summary>본문 한 문단을 다른 주제 또는 오류 그룹 텍스트로 교체한다. 실패 시 null.</summary>
        private ReportData ForgeReportBody(ReportSO.ReportGroup group, ReportData honest)
        {
            if (group == null || honest.ParagraphCount == 0)
                return null;

            ReportSO.ReportGroup other = _report.GetRandomOtherGroup(group);
            bool canWrongTopic = other != null;
            bool canError = _report.HasError;
            if (!canWrongTopic && !canError)
                return null;

            var keys = ExtractKeys(honest);
            int idx = Random.Range(0, keys.Count);

            bool useError = canError && (!canWrongTopic || Random.value < 0.5f);
            string replacement = useError ? _report.GetRandomErrorKey() : other.GetRandomParagraph();
            if (string.IsNullOrEmpty(replacement))
                return null;

            keys[idx] = replacement;
            return new ReportData(honest.TopicKey, keys, forgedParagraphIndex: idx);
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
