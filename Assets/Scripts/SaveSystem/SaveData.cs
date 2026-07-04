using System;

namespace MageAcademy.SaveSystem
{
    /// <summary>
    /// 저장되는 진행 데이터. 향후 항목 추가 시 필드만 늘리고 <see cref="version"/>을 올린다.
    /// JsonUtility로 직렬화하므로 public 필드만 사용한다.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>현재 스키마 버전. 향후 마이그레이션 분기용.</summary>
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;

        /// <summary>재개할 일차(1부터).</summary>
        public int currentDay = 1;

        /// <summary>마법 학교 평판.</summary>
        public int reputation;
    }
}
