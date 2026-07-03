using System.Collections.Generic;
using GuildGame.Core;
using GuildGame.Data;
using GuildGame.Gameplay.Managers;
using GuildGame.Gameplay.Models;
using GuildGame.Gameplay.Services;
using GuildGame.Localization;
using GuildGame.UI;
using UnityEngine;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 컴포지션 루트(진입점). 서비스·모델·상태를 조립하고 뷰에 컨텍스트를 주입한 뒤
    /// 상태 머신을 손님 입장 상태에서 시작한다. 사이클 진행은 상태들이 담당한다.
    /// </summary>
    public class GuildDeskController : MonoBehaviour
    {
        [SerializeField] private GameBalanceSO _balance;

        [Tooltip("손님 이름 로컬라이제이션 키 풀")]
        [SerializeField] private string[] _adventurerNameKeys =
        {
            "adv_name_0", "adv_name_1", "adv_name_2", "adv_name_3"
        };

        private StateMachine _machine;
        private GameContext _context;

        private void Start()
        {
            if (_balance == null)
            {
                Debug.LogError("[GuildDesk] GameBalanceSO가 할당되지 않음. 진행 불가.");
                enabled = false;
                return;
            }

            // ---- 의존성 조립 ----
            ILocalizationProvider localization = LocalizationManager.Instance;
            ITestimonyGenerator generator = new RandomTestimonyGenerator(_balance.lieChance, _adventurerNameKeys);
            IJudgementService judgement = new JudgementService();
            var reputation = new ReputationModel(_balance.startingReputation);

            _context = new GameContext(
                localization,
                generator,
                judgement,
                QuestManager.Instance,
                _balance,
                reputation);

            // ---- 상태 조립 및 선형 배선 ----
            _machine = new StateMachine();
            var enter = new AdventurerEnterState(_context, _machine);
            var inspection = new InspectionState(_context, _machine);
            var resolution = new ResolutionState(_context, _machine);

            enter.Next = inspection;
            inspection.Next = resolution;
            resolution.Next = enter; // 루프

            // ---- 뷰 바인딩 ----
            BindViews();

            // ---- 사이클 시작 ----
            _machine.ChangeState(enter);
        }

        private void Update()
        {
            _machine?.Tick();
        }

        private void BindViews()
        {
            var views = FindObjectsByType<UIViewBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var view in views)
                view.Bind(_context);
        }
    }
}
