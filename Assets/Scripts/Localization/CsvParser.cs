using System.Collections.Generic;
using System.Text;

namespace GuildGame.Localization
{
    /// <summary>
    /// RFC-4180 스타일의 최소 CSV 파서. 따옴표로 감싼 필드 내부의 콤마/개행/이스케이프된
    /// 따옴표("")를 처리한다. 로컬라이제이션 매니저와 분리해 순수 함수로 유지한다(SRP, 테스트 용이).
    /// </summary>
    public static class CsvParser
    {
        /// <summary>CSV 원문을 행 → 필드 배열의 목록으로 변환한다. 빈 줄은 건너뛰지 않는다.</summary>
        public static List<string[]> Parse(string text)
        {
            var rows = new List<string[]>();
            if (string.IsNullOrEmpty(text))
                return rows;

            var fields = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;
            int i = 0;
            int len = text.Length;

            while (i < len)
            {
                char c = text[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // 다음 문자가 따옴표면 이스케이프된 따옴표(""), 아니면 인용 종료.
                        if (i + 1 < len && text[i + 1] == '"')
                        {
                            sb.Append('"');
                            i += 2;
                            continue;
                        }
                        inQuotes = false;
                        i++;
                        continue;
                    }

                    sb.Append(c);
                    i++;
                    continue;
                }

                switch (c)
                {
                    case '"':
                        inQuotes = true;
                        i++;
                        break;
                    case ',':
                        fields.Add(sb.ToString());
                        sb.Clear();
                        i++;
                        break;
                    case '\r':
                        // CRLF / CR 모두 행 종료로 처리.
                        FlushRow(rows, fields, sb);
                        if (i + 1 < len && text[i + 1] == '\n')
                            i += 2;
                        else
                            i++;
                        break;
                    case '\n':
                        FlushRow(rows, fields, sb);
                        i++;
                        break;
                    default:
                        sb.Append(c);
                        i++;
                        break;
                }
            }

            // 마지막 개행이 없는 경우의 잔여 행 처리.
            if (sb.Length > 0 || fields.Count > 0)
                FlushRow(rows, fields, sb);

            return rows;
        }

        private static void FlushRow(List<string[]> rows, List<string> fields, StringBuilder sb)
        {
            fields.Add(sb.ToString());
            sb.Clear();
            rows.Add(fields.ToArray());
            fields.Clear();
        }
    }
}
