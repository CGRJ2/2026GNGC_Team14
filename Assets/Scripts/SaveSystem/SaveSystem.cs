using System;
using System.IO;
using UnityEngine;

namespace MageAcademy.SaveSystem
{
    /// <summary>
    /// 세이브 데이터의 파일 입출력을 담당하는 정적 유틸리티.
    /// Application.persistentDataPath/save.json 에 JsonUtility로 저장한다.
    /// 상태를 갖지 않으므로 씬 로드 전/후 어디서든 호출할 수 있다.
    /// </summary>
    public static class SaveSystem
    {
        private const string FileName = "save.json";

        private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        /// <summary>세이브 파일이 존재하는지.</summary>
        public static bool HasSave()
        {
            return File.Exists(FilePath);
        }

        /// <summary>진행 데이터를 JSON으로 저장한다.</summary>
        public static void Save(SaveData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[SaveSystem] null 데이터는 저장하지 않습니다.");
                return;
            }

            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(FilePath, json);
                Debug.Log($"[SaveSystem] 저장 완료: day={data.currentDay}, reputation={data.reputation}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] 저장 실패: {e.Message}");
            }
        }

        /// <summary>세이브를 읽어 반환한다. 없거나 손상되면 null.</summary>
        public static SaveData Load()
        {
            if (!File.Exists(FilePath))
                return null;

            try
            {
                string json = File.ReadAllText(FilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                if (data == null)
                    Debug.LogWarning("[SaveSystem] 세이브 파싱 결과가 null입니다.");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] 로드 실패: {e.Message}");
                return null;
            }
        }

        /// <summary>세이브 파일을 삭제한다(디버그/리셋용).</summary>
        public static void Delete()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    Debug.Log("[SaveSystem] 세이브 삭제 완료.");
                }
                else
                {
                    Debug.Log("[SaveSystem] 삭제할 세이브가 없습니다.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] 삭제 실패: {e.Message}");
            }
        }
    }
}
