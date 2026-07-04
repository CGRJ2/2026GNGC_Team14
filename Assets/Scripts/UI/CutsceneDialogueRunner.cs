using System.Collections;
using System.Collections.Generic;
using MageAcademy.Data;
using UnityEngine;

namespace MageAcademy.UI
{
    public class CutsceneDialogueRunner : UIViewBase
    {
        [SerializeField] private CutsceneSO _playOnBind;
        [SerializeField] private bool _autoPlayOnBind;
        [SerializeField] private bool _exitStudentOnEnd = true;

        private Coroutine _playRoutine;
        private string _currentStudentId;

        public bool IsPlaying => _playRoutine != null;

        protected override void OnBind()
        {
            Context.CutscenePlayRequested += Play;

            if (_autoPlayOnBind && _playOnBind != null)
                Play(_playOnBind);
        }

        public void Play(CutsceneSO cutscene)
        {
            if (cutscene == null || Context == null)
                return;

            Stop(raiseEnded: false);
            Context.RaiseCutsceneStarted();
            _playRoutine = StartCoroutine(PlayRoutine(cutscene));
        }

        public void Stop()
        {
            Stop(raiseEnded: true);
        }

        private void Stop(bool raiseEnded)
        {
            if (_playRoutine == null)
                return;

            StopCoroutine(_playRoutine);
            _playRoutine = null;
            ExitCurrentStudent();
            if (raiseEnded)
                Context?.RaiseCutsceneEnded();
        }

        private IEnumerator PlayRoutine(CutsceneSO cutscene)
        {
            foreach (CutsceneSO.Line line in cutscene.Lines)
            {
                if (line == null)
                    continue;

                if (line.stepType == CutsceneStepType.StudentExit)
                {
                    ExitCurrentStudent(force: true);
                    yield return new WaitForSeconds(GetStudentExitDelay());
                    yield return new WaitForSeconds(GetLineDelay(line));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line.localizationKey))
                    continue;

                if (line.speaker == CutsceneSpeaker.Student)
                    yield return EnterStudentIfNeeded(line.studentId);

                IReadOnlyList<string> texts = Context.Localization.GetAll(line.localizationKey);
                for (int i = 0; i < texts.Count; i++)
                {
                    Context.RaiseCutsceneDialogue(line.speaker, texts[i]);
                    yield return new WaitForSeconds(GetLineDelay(line));
                }
            }

            if (_exitStudentOnEnd && !string.IsNullOrEmpty(_currentStudentId))
            {
                ExitCurrentStudent(force: true);
                yield return new WaitForSeconds(GetStudentExitDelay());
            }

            _playRoutine = null;
            Context.RaiseCutsceneEnded();
        }

        private IEnumerator EnterStudentIfNeeded(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId) || studentId == _currentStudentId)
                yield break;

            StudentSO student = Context.StudentDatabase != null
                ? Context.StudentDatabase.FindById(studentId)
                : null;

            if (student == null)
            {
                Debug.LogWarning($"[Cutscene] Student '{studentId}' was not found in StudentDatabaseSO.");
                yield break;
            }

            if (!string.IsNullOrEmpty(_currentStudentId))
            {
                ExitCurrentStudent(force: true);
                yield return new WaitForSeconds(GetStudentExitDelay());
            }

            _currentStudentId = studentId;
            Context.RaiseCutsceneStudentEnter(student);
            yield return new WaitForSeconds(GetStudentEnterDelay());
        }

        private void ExitCurrentStudent(bool force = false)
        {
            if (!force && string.IsNullOrEmpty(_currentStudentId))
                return;

            _currentStudentId = null;
            Context?.RaiseCutsceneStudentExit();
        }

        private float GetLineDelay(CutsceneSO.Line line)
        {
            if (line.delayAfter >= 0f)
                return line.delayAfter;

            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            return settings != null ? settings.cutsceneLineDelay : 1.0f;
        }

        private float GetStudentEnterDelay()
        {
            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            if (settings == null || settings.studentEnter == null)
                return 0.4f;

            return settings.studentDoorOpenLeadDelay + settings.studentEnter.delay + settings.studentEnter.duration;
        }

        private float GetStudentExitDelay()
        {
            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            if (settings == null || settings.studentExit == null)
                return 0.4f;

            return settings.studentExit.delay + settings.studentExit.duration;
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CutscenePlayRequested -= Play;

            if (_playRoutine != null)
                StopCoroutine(_playRoutine);

            _playRoutine = null;
            _currentStudentId = null;
        }
    }
}
