namespace MageAcademy.Data
{
    /// <summary>
    /// 학생증에서 검증하는 항목. 앞의 4개는 텍스트(질문/증언으로 대조),
    /// FacePhoto는 사진(실제 학생 일러스트와 시각 대조).
    /// </summary>
    public enum StudentIdFieldType
    {
        Name,
        EnrollmentDate,
        Grade,
        Major,
        FacePhoto
    }
}
