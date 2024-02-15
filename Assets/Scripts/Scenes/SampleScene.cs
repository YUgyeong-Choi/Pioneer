/// <summary>
/// Scene의 기본 GameObject가 가져야 할 스크립트의 샘플.
/// 이 클래스는 BaseScene을 상속받아, 특정 씬에 필요한 초기화 및 정리 작업을 구현함.
/// </summary>
public class SampleScene : BaseScene
{
    // Init 메서드는 씬의 초기화 로직을 구현함.
    // 모든 상속받은 씬 클래스에서는 이 메서드를 오버라이드해야 하며,
    // base.Init()을 호출하여 BaseScene의 초기화 로직을 먼저 실행해야 합니다.
    protected override void Init()
    {
        base.Init();                    // BaseScene의 Init 메서드 호출

        SceneType = Define.Scene.Game; // 현재 씬의 타입을 Game으로 설정
        
        // 추가적인 씬 초기화 로직을 여기에 구현할 수 있음.
        // 예를 들어, 씬에 필요한 UI를 표시하거나,
        // Managers를 통해 데이터를 로드하는 등의 작업을 수행할 수 있음.
        
        // Managers.UI.ShowSceneUI<UI_Inven>();                         // 예시: 인벤토리 UI를 표시
        // Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;    // 예시: 데이터 매니저로부터 Stat 데이터를 로드
    }
    
    // Clear 메서드는 씬이 파괴될 때 호출되어야 하는 정리 작업을 구현.
    // BaseScene에서는 이 메서드가 abstract로 선언되어 있으므로,
    // 모든 상속받은 씬 클래스에서는 이 메서드를 구현해야 함.
    public override void Clear()
    {
        // 씬이 파괴될 때 필요한 정리 작업을 여기에 구현.
        // 예를 들어, 씬에서 사용된 리소스를 해제하거나,
        // Managers를 통해 설정된 데이터를 초기화하는 등의 작업을 수행할 수 있음.
    }

}