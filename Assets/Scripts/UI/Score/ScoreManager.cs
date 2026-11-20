/// <summary>
/// 게임 전체의 점수
/// </summary>
public static class ScoreManager
{
    public static int TotalScore { get; private set; } = 0;
    public static void AddScore(int InScore)
    {
        if (InScore <= 0)
        {
            return;
        }

        TotalScore += InScore;
    }
    
    public static void ResetScore()
    {
        TotalScore = 0;
    }
}