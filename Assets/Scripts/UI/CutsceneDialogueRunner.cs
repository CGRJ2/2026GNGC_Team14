using System.Collections;
using GuildGame.Data;
using UnityEngine;

namespace GuildGame.UI
{
    public class CutsceneDialogueRunner : UIViewBase
    {
        [SerializeField] private CutsceneSO _playOnBind;
        [SerializeField] private bool _autoPlayOnBind;

        private Coroutine _playRoutine;

        public bool IsPlaying => _playRoutine != null;

        protected override void OnBind()
        {
            if (_autoPlayOnBind && _playOnBind != null)
                Play(_playOnBind);
        }

        public void Play(CutsceneSO cutscene)
        {
            if (cutscene == null || Context == null)
                return;

            Stop();
            _playRoutine = StartCoroutine(PlayRoutine(cutscene));
        }

        public void Stop()
        {
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }

            Context?.RaiseCutsceneEnded();
        }

        private IEnumerator PlayRoutine(CutsceneSO cutscene)
        {
            foreach (CutsceneSO.Line line in cutscene.Lines)
            {
                if (line == null || string.IsNullOrWhiteSpace(line.localizationKey))
                    continue;

                string text = line.useRandomVariant
                    ? Context.Localization.GetRandom(line.localizationKey)
                    : Context.Localization.Get(line.localizationKey);

                Context.RaiseCutsceneDialogue(line.speaker, text);
                yield return new WaitForSeconds(GetLineDelay(line));
            }

            _playRoutine = null;
            Context.RaiseCutsceneEnded();
        }

        private float GetLineDelay(CutsceneSO.Line line)
        {
            if (line.delayAfter >= 0f)
                return line.delayAfter;

            UIAnimationSettingsSO settings = Context.UIAnimationSettings;
            return settings != null ? settings.cutsceneLineDelay : 1.0f;
        }

        private void OnDestroy()
        {
            if (_playRoutine != null)
                StopCoroutine(_playRoutine);

            _playRoutine = null;
        }
    }
}
