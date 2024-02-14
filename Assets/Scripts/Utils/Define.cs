/// <summary>
/// 프로젝트의 모든 영역에서 사용되는 enum들을 정의하는 클래스. 
/// </summary>
public class Define
{
    /// <summary>
    /// Layer를 구분하기 위한 enum. ray cast를 사용할 때 활용
    /// </summary>
    public enum Layer
    {
    }
    
    /// <summary>
    /// Scene을 구분하기 위한 enum. 새로운 Scene이 추가될 때 마다 동일한 이름을 이 enum 안에 선언해야 함.
    /// </summary>
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    /// <summary>
    /// Sound의 종류를 구분하기 위한 enum. 배경음악과 효과음으로 구분.
    /// </summary>
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }
}
