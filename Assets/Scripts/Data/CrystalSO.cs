using System;
using System.Collections.Generic;
using UnityEngine;

namespace MageAcademy.Data
{
    /// <summary>
    /// 수정구슬(알리바이) 케이스 config. 각 케이스는 "본인이 직접 했다"는 주장 질문과,
    /// 뒷받침 장면(정직)·모순 장면(거짓) 및 각 진술을 가진다.
    /// 실제 텍스트는 Localization.csv, 장면은 스프라이트로 지정한다.
    /// </summary>
    [CreateAssetMenu(fileName = "Crystal", menuName = "MageAcademy/Crystal", order = 43)]
    public class CrystalSO : ScriptableObject
    {
        [Serializable]
        public class CrystalCase
        {
            [Tooltip("질문(예: 이 과제 네가 직접 한 거 맞지?)")]
            public string questionKey;

            [Tooltip("정직할 때 진술(주장)")]
            public string honestTestimonyKey;

            [Tooltip("거짓일 때 진술(주장)")]
            public string lyingTestimonyKey;

            [Tooltip("정직 장면: 실제로 하는 모습(작업중/도서관)")]
            public Sprite honestScene;

            [Tooltip("거짓 장면: 안 하는 모습(놀기/딴 곳)")]
            public Sprite lyingScene;
        }

        [Tooltip("수정구슬 알리바이 케이스 풀")]
        public List<CrystalCase> cases = new();

        public bool HasCases => cases != null && cases.Count > 0;

        public CrystalCase GetRandomCase()
        {
            return HasCases ? cases[UnityEngine.Random.Range(0, cases.Count)] : null;
        }
    }
}
