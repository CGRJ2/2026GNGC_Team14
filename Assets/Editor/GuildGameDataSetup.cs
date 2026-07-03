using System.Collections.Generic;
using System.IO;
using GuildGame.Data;
using UnityEditor;
using UnityEngine;

namespace GuildGame.EditorTools
{
    /// <summary>
    /// MVP용 샘플 데이터(의뢰/질문/데이터베이스/밸런스)를 코드로 일괄 생성한다.
    /// 메뉴: GuildGame/1. Create Sample Data. GUID 수작업 없이 참조까지 배선한다.
    /// </summary>
    public static class GuildGameDataSetup
    {
        private const string DataRoot = "Assets/Data";
        private const string QuestDir = DataRoot + "/Quests";
        private const string QuestionDir = DataRoot + "/Questions";
        private const string DatabaseDir = DataRoot + "/Databases";
        private const string BalanceDir = DataRoot + "/Balance";

        [MenuItem("GuildGame/1. Create Sample Data")]
        public static void CreateSampleData()
        {
            EnsureFolders();

            // ---- 질문 4종 ----
            var qName = CreateQuestion("q_target_name", QuestFactType.TargetName, "q_target_name", "a_target_name");
            var qCount = CreateQuestion("q_target_count", QuestFactType.TargetCount, "q_target_count", "a_target_count");
            var qLoc = CreateQuestion("q_location", QuestFactType.Location, "q_location", "a_location");
            var qDiff = CreateQuestion("q_difficulty", QuestFactType.Difficulty, "q_difficulty", "a_difficulty");

            var questionDb = CreateOrReplace<QuestionDatabaseSO>($"{DatabaseDir}/QuestionDatabase.asset");
            questionDb.questions = new List<QuestionSO> { qName, qCount, qLoc, qDiff };
            EditorUtility.SetDirty(questionDb);

            // ---- 의뢰 3종 ----
            var goblin = CreateQuest("Quest_Goblin", "quest_goblin", "quest_goblin_title", "quest_goblin_summary",
                ("val_goblin_true", "val_goblin_lie"),
                ("val_goblin_count_true", "val_goblin_count_lie"),
                ("val_goblin_loc_true", "val_goblin_loc_lie"),
                ("val_goblin_diff_true", "val_goblin_diff_lie"));

            var herb = CreateQuest("Quest_Herb", "quest_herb", "quest_herb_title", "quest_herb_summary",
                ("val_herb_true", "val_herb_lie"),
                ("val_herb_count_true", "val_herb_count_lie"),
                ("val_herb_loc_true", "val_herb_loc_lie"),
                ("val_herb_diff_true", "val_herb_diff_lie"));

            var wolf = CreateQuest("Quest_Wolf", "quest_wolf", "quest_wolf_title", "quest_wolf_summary",
                ("val_wolf_true", "val_wolf_lie"),
                ("val_wolf_count_true", "val_wolf_count_lie"),
                ("val_wolf_loc_true", "val_wolf_loc_lie"),
                ("val_wolf_diff_true", "val_wolf_diff_lie"));

            var questDb = CreateOrReplace<QuestDatabaseSO>($"{DatabaseDir}/QuestDatabase.asset");
            questDb.quests = new List<QuestDataSO> { goblin, herb, wolf };
            EditorUtility.SetDirty(questDb);

            // ---- 밸런스 ----
            CreateOrReplace<GameBalanceSO>($"{BalanceDir}/GameBalance.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[GuildGameSetup] 샘플 데이터 생성 완료: 의뢰 3, 질문 4, DB 2, 밸런스 1.");
        }

        private static QuestionSO CreateQuestion(string id, QuestFactType fact, string textKey, string answerKey)
        {
            var q = CreateOrReplace<QuestionSO>($"{QuestionDir}/{id}.asset");
            q.questionId = id;
            q.targetFact = fact;
            q.questionTextKey = textKey;
            q.answerTemplateKey = answerKey;
            EditorUtility.SetDirty(q);
            return q;
        }

        private static QuestDataSO CreateQuest(
            string fileName, string questId, string titleKey, string summaryKey,
            (string t, string l) target,
            (string t, string l) count,
            (string t, string l) location,
            (string t, string l) difficulty)
        {
            var quest = CreateOrReplace<QuestDataSO>($"{QuestDir}/{fileName}.asset");
            quest.questId = questId;
            quest.titleKey = titleKey;
            quest.summaryKey = summaryKey;
            quest.facts = new List<QuestFact>
            {
                new() { type = QuestFactType.TargetName, trueValueKey = target.t, lieValueKey = target.l },
                new() { type = QuestFactType.TargetCount, trueValueKey = count.t, lieValueKey = count.l },
                new() { type = QuestFactType.Location, trueValueKey = location.t, lieValueKey = location.l },
                new() { type = QuestFactType.Difficulty, trueValueKey = difficulty.t, lieValueKey = difficulty.l },
            };
            EditorUtility.SetDirty(quest);
            return quest;
        }

        private static T CreateOrReplace<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
                return existing;

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "Data");
            EnsureFolder(DataRoot, "Quests");
            EnsureFolder(DataRoot, "Questions");
            EnsureFolder(DataRoot, "Databases");
            EnsureFolder(DataRoot, "Balance");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string full = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(full))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
