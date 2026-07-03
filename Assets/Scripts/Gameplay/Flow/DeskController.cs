using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GuildGame.Core;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using GuildGame.Gameplay.Services;
using GuildGame.Localization;
using GuildGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 학생증 검증 루프의 composition root.
    /// 일반 모드에서는 학생을 랜덤 생성하고, 튜토리얼 모드에서는 지정 학생 1명으로 고정 시퀀스를 진행한다.
    /// </summary>
    public class DeskController : MonoBehaviour
    {
        [SerializeField] private GameBalanceSO _balance;
        [SerializeField] private StudentDatabaseSO _studentDatabase;
        [SerializeField] private UIAnimationSettingsSO _uiAnimationSettings;
        [SerializeField] private DayScheduleSO _daySchedule;

        [Header("Tutorial")]
        [SerializeField] private bool _runTutorialOnStart;
        [SerializeField] private StudentSO _tutorialStudent;
        [SerializeField] private Sprite _tutorialCardPhoto;
        [SerializeField] private float _tutorialAfterAnswerDelay = 0.8f;
        [SerializeField] private float _tutorialPhotoReactionDelay = 0.8f;

        private StateMachine _machine;
        private GameContext _context;
        private TutorialHighlight _tutorialHighlight;

        private void Start()
        {
            if (_balance == null || _studentDatabase == null || _daySchedule == null)
            {
                Debug.LogError("[GuildDesk] GameBalanceSO, StudentDatabaseSO or DayScheduleSO is missing.");
                enabled = false;
                return;
            }

            ILocalizationProvider localization = LocalizationManager.Instance;
            IStudentCaseGenerator generator = CreateCaseGenerator();
            IJudgementService judgement = new JudgementService();
            var reputation = new ReputationModel(_balance.startingReputation);
            var day = new DayModel(_daySchedule);

            _context = new GameContext(
                localization,
                generator,
                judgement,
                _balance,
                reputation,
                day,
                _uiAnimationSettings,
                loopAfterResolution: !_runTutorialOnStart,
                isTutorial: _runTutorialOnStart);

            _machine = new StateMachine();
            var dayStart = new DayStartState(_context, _machine);
            var enter = new StudentEnterState(_context, _machine);
            var inspection = new InspectionState(_context, _machine);
            var resolution = new ResolutionState(_context, _machine);
            var dayEnd = new DayEndState(_context, _machine);

            dayStart.Next = enter;
            enter.Next = inspection;
            inspection.Next = resolution;
            resolution.Next = enter;
            resolution.DayEnd = dayEnd;
            dayEnd.Next = dayStart;

            BindViews();
            _machine.ChangeState(_runTutorialOnStart ? (IState)enter : dayStart);

            if (_runTutorialOnStart)
                StartCoroutine(RunTutorialSequence());
        }

        private void Update()
        {
            _machine?.Tick();
        }

        private void OnDestroy()
        {
            _tutorialHighlight?.Stop();
        }

        private void BindViews()
        {
            var views = FindObjectsByType<UIViewBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var view in views)
                view.Bind(_context);
        }

        private IStudentCaseGenerator CreateCaseGenerator()
        {
            if (!_runTutorialOnStart)
                return new RandomStudentCaseGenerator(_studentDatabase, _balance.lieChance);

            StudentSO tutorialStudent = _tutorialStudent != null
                ? _tutorialStudent
                : _studentDatabase.FindById("std_tutorial");

            return new TutorialStudentCaseGenerator(tutorialStudent, _tutorialCardPhoto);
        }

        private IEnumerator RunTutorialSequence()
        {
            var table = FindFirstObjectByType<PanelTableController>(FindObjectsInactive.Include);
            var idPanel = FindFirstObjectByType<StudentIdPanelView>(FindObjectsInactive.Include);
            var verdict = FindFirstObjectByType<VerdictButtonsView>(FindObjectsInactive.Include);

            if (table == null || idPanel == null || verdict == null)
            {
                Debug.LogError("[Tutorial] Tutorial UI references are missing.");
                yield break;
            }

            _tutorialHighlight = new TutorialHighlight();

            idPanel.SetAllFieldButtonsInteractable(false);
            verdict.SetButtonsInteractable(false, false);
            table.SetStudentIdButtonInteractable(false);

            float enterDelay = _uiAnimationSettings != null
                ? Mathf.Max(
                    _uiAnimationSettings.studentEnter.delay + _uiAnimationSettings.studentEnter.duration,
                    _uiAnimationSettings.studentIdButton.delay + _uiAnimationSettings.studentIdButton.duration)
                : 0.5f;
            yield return new WaitForSeconds(enterDelay);

            bool opened = table.StudentIdPanel != null && table.StudentIdPanel.activeInHierarchy;
            void OnOpened() => opened = true;

            table.StudentIdPanelOpened += OnOpened;
            table.SetStudentIdButtonInteractable(true);
            _tutorialHighlight.Play(table.StudentIdButton != null ? table.StudentIdButton.gameObject : null);

            yield return new WaitUntil(() => opened);

            table.StudentIdPanelOpened -= OnOpened;
            _tutorialHighlight.Stop();
            table.SetStudentIdButtonInteractable(false);

            yield return RunFieldStep(idPanel, StudentIdFieldType.Name);
            yield return RunFieldStep(idPanel, StudentIdFieldType.EnrollmentDate);
            yield return RunFieldStep(idPanel, StudentIdFieldType.Grade);
            yield return RunFieldStep(idPanel, StudentIdFieldType.Major);
            yield return RunPhotoStep(idPanel, verdict);
        }

        private IEnumerator RunFieldStep(StudentIdPanelView idPanel, StudentIdFieldType field)
        {
            bool answered = false;
            void OnAnswered(string question, string answer) => answered = true;

            idPanel.SetOnlyFieldButtonInteractable(field);
            Button button = idPanel.GetButton(field);
            _tutorialHighlight.Play(button != null ? button.gameObject : null);

            _context.AnswerGiven += OnAnswered;
            yield return new WaitUntil(() => answered);
            _context.AnswerGiven -= OnAnswered;

            _tutorialHighlight.Stop();
            idPanel.SetAllFieldButtonsInteractable(false);
            yield return new WaitForSeconds(_tutorialAfterAnswerDelay);
        }

        private IEnumerator RunPhotoStep(StudentIdPanelView idPanel, VerdictButtonsView verdict)
        {
            bool questionAsked = false;
            void OnQuestionAsked(string question) => questionAsked = true;

            idPanel.SetOnlyFieldButtonInteractable(StudentIdFieldType.FacePhoto);
            Button photoButton = idPanel.GetButton(StudentIdFieldType.FacePhoto);
            _tutorialHighlight.Play(photoButton != null ? photoButton.gameObject : null);

            _context.QuestionAsked += OnQuestionAsked;
            yield return new WaitUntil(() => questionAsked);
            _context.QuestionAsked -= OnQuestionAsked;

            _tutorialHighlight.Stop();
            idPanel.SetAllFieldButtonsInteractable(false);
            yield return new WaitForSeconds(_tutorialPhotoReactionDelay);

            bool resolved = false;
            void OnOutcomeResolved(CaseOutcome outcome, string eventText) => resolved = true;

            _context.OutcomeResolved += OnOutcomeResolved;
            verdict.SetButtonsInteractable(false, true);
            _tutorialHighlight.Play(verdict.FailButton != null ? verdict.FailButton.gameObject : null);

            yield return new WaitUntil(() => resolved);

            _context.OutcomeResolved -= OnOutcomeResolved;
            _tutorialHighlight.Stop();
            verdict.SetButtonsInteractable(false, false);
        }

        private sealed class TutorialStudentCaseGenerator : IStudentCaseGenerator
        {
            private readonly StudentSO _student;
            private readonly Sprite _cardPhoto;

            public TutorialStudentCaseGenerator(StudentSO student, Sprite cardPhoto)
            {
                _student = student;
                _cardPhoto = cardPhoto;
            }

            public StudentCase Generate()
            {
                if (_student == null)
                    return null;

                var forged = new Dictionary<StudentIdFieldType, bool>
                {
                    [StudentIdFieldType.Name] = false,
                    [StudentIdFieldType.EnrollmentDate] = false,
                    [StudentIdFieldType.Grade] = false,
                    [StudentIdFieldType.Major] = false,
                    [StudentIdFieldType.FacePhoto] = true,
                };

                var cardText = new Dictionary<StudentIdFieldType, string>
                {
                    [StudentIdFieldType.Name] = _student.GetText(StudentIdFieldType.Name),
                    [StudentIdFieldType.EnrollmentDate] = _student.GetText(StudentIdFieldType.EnrollmentDate),
                    [StudentIdFieldType.Grade] = _student.GetText(StudentIdFieldType.Grade),
                    [StudentIdFieldType.Major] = _student.GetText(StudentIdFieldType.Major),
                };

                Sprite cardPhoto = _cardPhoto != null ? _cardPhoto : _student.IdPhoto;
                return new StudentCase(_student, forged, cardText, cardPhoto);
            }
        }

        private sealed class TutorialHighlight
        {
            private Tween _tween;
            private Transform _target;
            private Vector3 _originalScale;
            private Graphic _graphic;
            private Color _originalColor;

            public void Play(GameObject target)
            {
                Stop();
                if (target == null)
                    return;

                _target = target.transform;
                _originalScale = _target.localScale;
                _graphic = target.GetComponent<Graphic>();
                if (_graphic != null)
                    _originalColor = _graphic.color;

                var sequence = DOTween.Sequence();
                sequence.Join(_target.DOScale(_originalScale * 1.08f, 0.35f).SetLoops(-1, LoopType.Yoyo));
                if (_graphic != null)
                {
                    var highlightColor = new Color(1f, 0.86f, 0.25f, _originalColor.a);
                    sequence.Join(_graphic.DOColor(highlightColor, 0.35f).SetLoops(-1, LoopType.Yoyo));
                }

                _tween = sequence;
            }

            public void Stop()
            {
                _tween?.Kill();
                _tween = null;

                if (_target != null)
                    _target.localScale = _originalScale;
                if (_graphic != null)
                    _graphic.color = _originalColor;

                _target = null;
                _graphic = null;
            }
        }
    }
}
