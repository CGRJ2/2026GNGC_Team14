using System.Collections.Generic;
using MageAcademy.Data;
using MageAcademy.Gameplay.Models;
using UnityEngine;

namespace MageAcademy.Gameplay.Services
{
    /// <summary>
    /// 랜덤 학생을 뽑고 위조를 적용한다.
    /// - 검증면(학생증/레포트/수정구슬)이 그날 활성인지에 따라, 위조 케이스는 활성 축 중 한 곳만 위조한다.
    ///   레포트 본문 위조는 한 문단을 다른 주제/오류로, 수정구슬 위조는 진술과 모순되는 장면(거짓말)으로.
    /// </summary>
    public class RandomStudentCaseGenerator : IStudentCaseGenerator, IStudentCaseGeneratorLifecycle
    {
        private enum ForgeAxis { IdField, ReportBody, CrystalLie, UVGolem }

        private static readonly StudentIdFieldType[] AllFields =
        {
            StudentIdFieldType.Name,
            StudentIdFieldType.Grade,
            StudentIdFieldType.Major,
            StudentIdFieldType.FacePhoto
        };

        private readonly StudentDatabaseSO _database;
        private readonly GameBalanceSO _balance;
        private readonly float _forgeChance;
        private readonly ReportSO _report;
        private readonly CrystalSO _crystal;
        private readonly UVSO _uv;
        private readonly HashSet<StudentSO> _usedStudentsToday = new();

        public RandomStudentCaseGenerator(StudentDatabaseSO database, GameBalanceSO balance,
            ReportSO report = null, CrystalSO crystal = null, UVSO uv = null)
        {
            _database = database;
            _balance = balance;
            _forgeChance = balance != null ? Mathf.Clamp01(balance.lieChance) : 0f;
            _report = report;
            _crystal = crystal;
            _uv = uv;
        }

        public void BeginDay(int day)
        {
            _usedStudentsToday.Clear();
        }

        private float AxisWeight(ForgeAxis axis)
        {
            if (_balance == null)
                return 1f;
            switch (axis)
            {
                case ForgeAxis.IdField: return _balance.forgeWeightId;
                case ForgeAxis.ReportBody: return _balance.forgeWeightReport;
                case ForgeAxis.CrystalLie: return _balance.forgeWeightCrystal;
                case ForgeAxis.UVGolem: return _balance.forgeWeightUV;
                default: return 1f;
            }
        }

        public StudentCase Generate(bool includeReport, bool includeCrystal, bool includeUV)
        {
            StudentSO real = PickUnusedStudent();
            if (real == null)
                return null;

            _usedStudentsToday.Add(real);

            var forged = new Dictionary<StudentIdFieldType, bool>();
            var cardText = new Dictionary<StudentIdFieldType, string>();
            foreach (var field in AllFields)
            {
                forged[field] = false;
                if (field != StudentIdFieldType.FacePhoto)
                    cardText[field] = real.GetText(field);
            }
            Sprite cardPhoto = real.IdPhoto;

            // 정직 상태의 레포트/수정구슬을 먼저 만든다(포함 날 & 데이터 존재 시).
            ReportSO.ReportGroup group = null;
            ReportData report = null;
            if (includeReport && _report != null && _report.HasGroups)
            {
                group = _report.GetRandomGroup();
                report = BuildHonestReport(group);
            }

            CrystalSO.CrystalCase crystalCase = null;
            CrystalData crystal = null;
            if (includeCrystal && _crystal != null && _crystal.HasCases)
            {
                crystalCase = _crystal.GetRandomCase();
                crystal = BuildHonestCrystal(crystalCase);
            }

            UVData uv = null;
            if (includeUV && _uv != null)
                uv = BuildHonestUV();

            if (Random.value < _forgeChance)
            {
                if (report != null || crystal != null || uv != null)
                    ForgeSingleTell(real, forged, cardText, ref cardPhoto,
                        group, ref report, crystalCase, ref crystal, ref uv);
                else
                    ForgeIdFields(real, forged, cardText, ref cardPhoto, Random.Range(1, 3));
            }

            return new StudentCase(real, forged, cardText, cardPhoto, report, crystal, uv);
        }

        private StudentSO PickUnusedStudent()
        {
            if (_database == null || _database.IsEmpty)
                return null;

            int count = _database.Count;
            for (int i = 0; i < count * 2; i++)
            {
                StudentSO candidate = _database.GetRandom();
                if (candidate != null && !_usedStudentsToday.Contains(candidate))
                    return candidate;
            }

            foreach (StudentSO student in _database.students)
            {
                if (student != null && !_usedStudentsToday.Contains(student))
                    return student;
            }

            Debug.LogWarning("[StudentCase] Student pool exhausted for this day. Reusing a student.");
            return _database.GetRandom();
        }

        /// <summary>활성 축 {학생증 / 레포트 / 수정구슬 / UV} 중 실현 가능한 한 곳만 위조한다.</summary>
        private void ForgeSingleTell(
            StudentSO real,
            Dictionary<StudentIdFieldType, bool> forged,
            Dictionary<StudentIdFieldType, string> cardText,
            ref Sprite cardPhoto,
            ReportSO.ReportGroup group, ref ReportData report,
            CrystalSO.CrystalCase crystalCase, ref CrystalData crystal,
            ref UVData uv)
        {
            var axes = new List<ForgeAxis> { ForgeAxis.IdField };
            if (report != null) axes.Add(ForgeAxis.ReportBody);
            if (crystal != null) axes.Add(ForgeAxis.CrystalLie);
            if (uv != null) axes.Add(ForgeAxis.UVGolem);
            WeightedOrder(axes);

            foreach (var axis in axes)
            {
                if (axis == ForgeAxis.IdField)
                {
                    if (ForgeIdFields(real, forged, cardText, ref cardPhoto, 1) > 0)
                        return;
                }
                else if (axis == ForgeAxis.ReportBody)
                {
                    ReportData bodyForged = ForgeReportBody(group, report);
                    if (bodyForged != null) { report = bodyForged; return; }
                }
                else if (axis == ForgeAxis.CrystalLie)
                {
                    CrystalData lie = ForgeCrystalLie(crystalCase);
                    if (lie != null) { crystal = lie; return; }
                }
                else // UVGolem
                {
                    UVData golem = ForgeUVGolem();
                    if (golem != null) { uv = golem; return; }
                }
            }
            // 어떤 축도 위조 못 하면 정직 유지(희귀).
        }

        private UVData BuildHonestUV()
        {
            // 골렘 작품이 아니면 흔적 없음.
            return new UVData(isGolemWork: false, signatureSprite: null,
                questionKey: _uv.questionKey, reactionKey: string.Empty);
        }

        private UVData ForgeUVGolem()
        {
            if (_uv == null || !_uv.HasSignatures)
                return null;
            return new UVData(
                isGolemWork: true,
                signatureSprite: _uv.GetRandomSignature(),
                questionKey: _uv.questionKey,
                reactionKey: _uv.GetRandomReactionKey());
        }

        private CrystalData BuildHonestCrystal(CrystalSO.CrystalCase c)
        {
            return new CrystalData(c.questionKey, c.honestTestimonyKey, c.honestScene, isLie: false, c.honestAnimationKey);
        }

        private CrystalData ForgeCrystalLie(CrystalSO.CrystalCase c)
        {
            if (c == null)
                return null;
            return new CrystalData(c.questionKey, c.lyingTestimonyKey, c.lyingScene, isLie: true, c.lyingAnimationKey);
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

        /// <summary>축을 가중치 기반 무작위 순서로 재배열한다(가중치가 클수록 앞에 올 확률이 높음).</summary>
        private void WeightedOrder(List<ForgeAxis> axes)
        {
            // Efraimidis-Spirakis 가중 표본추출: key = u^(1/weight), key가 큰 순.
            var keyed = new List<KeyValuePair<ForgeAxis, float>>(axes.Count);
            foreach (ForgeAxis a in axes)
            {
                float w = Mathf.Max(0.0001f, AxisWeight(a));
                float u = Mathf.Clamp(Random.value, 1e-6f, 1f);
                keyed.Add(new KeyValuePair<ForgeAxis, float>(a, Mathf.Pow(u, 1f / w)));
            }
            keyed.Sort((x, y) => y.Value.CompareTo(x.Value));
            for (int i = 0; i < axes.Count; i++)
                axes[i] = keyed[i].Key;
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
