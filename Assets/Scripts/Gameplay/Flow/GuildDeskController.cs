using GuildGame.Core;
using GuildGame.Data;
using GuildGame.Gameplay.Models;
using GuildGame.Gameplay.Services;
using GuildGame.Localization;
using GuildGame.UI;
using UnityEngine;

namespace GuildGame.Gameplay.Flow
{
    /// <summary>
    /// 컴포지션 루트(진입점). 학생증 검증 서비스·모델·상태를 조립하고 뷰에 컨텍스트를 주입한 뒤
    /// 상태 머신을 학생 입장 상태에서 시작한다.
    /// </summary>
    public class GuildDeskController : MonoBehaviour
    {
        [SerializeField] private GameBalanceSO _balance;
        [SerializeField] private StudentDatabaseSO _studentDatabase;

        private StateMachine _machine;
        private GameContext _context;

        private void Start()
        {
            if (_balance == null || _studentDatabase == null)
            {
                Debug.LogError("[GuildDesk] GameBalanceSO 또는 StudentDatabaseSO 미할당. 진행 불가.");
                enabled = false;
                return;
            }

            // ---- 의존성 조립 ----
            ILocalizationProvider localization = LocalizationManager.Instance;
            IStudentCaseGenerator generator = new RandomStudentCaseGenerator(_studentDatabase, _balance.lieChance);
            IJudgementService judgement = new JudgementService();
            var reputation = new ReputationModel(_balance.startingReputation);

            _context = new GameContext(localization, generator, judgement, _balance, reputation);

            // ---- 상태 조립 및 선형 배선 ----
            _machine = new StateMachine();
            var enter = new StudentEnterState(_context, _machine);
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
