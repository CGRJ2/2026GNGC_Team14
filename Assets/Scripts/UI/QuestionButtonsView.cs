using System.Collections.Generic;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildGame.UI
{
    /// <summary>
    /// 현재 의뢰의 검증 항목에 대응하는 질문 버튼을 동적으로 생성한다.
    /// 클릭 시 컨텍스트에 질문을 요청한다(질문 무제한).
    /// </summary>
    public class QuestionButtonsView : UIViewBase
    {
        [SerializeField] private Transform _container;
        [SerializeField] private Button _buttonPrefab;

        private readonly List<Button> _spawned = new();

        protected override void OnBind()
        {
            Context.CaseStarted += OnCaseStarted;
        }

        private void OnCaseStarted(AdventurerCase adventurerCase)
        {
            ClearButtons();

            if (adventurerCase == null || adventurerCase.Quest == null)
                return;
            if (_container == null || _buttonPrefab == null)
                return;

            List<QuestionSO> questions = Context.Quests.GetQuestionsFor(adventurerCase.Quest);
            foreach (var question in questions)
                CreateButton(question);
        }

        private void CreateButton(QuestionSO question)
        {
            Button button = Instantiate(_buttonPrefab, _container);
            button.gameObject.SetActive(true);

            var label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = Context.Localization.Get(question.questionTextKey);

            QuestionSO captured = question; // 클로저 캡처
            button.onClick.AddListener(() => Context.RequestQuestion(captured));

            _spawned.Add(button);
        }

        private void ClearButtons()
        {
            foreach (var button in _spawned)
            {
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    Destroy(button.gameObject);
                }
            }
            _spawned.Clear();
        }

        private void OnDestroy()
        {
            if (Context != null)
                Context.CaseStarted -= OnCaseStarted;
        }
    }
}
