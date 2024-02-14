/// <summary>
/// Scene의 기본 GameObject가 가져야 할 스크립트의 샘플.
/// (1) Init() 함수를 override해야하고, 그 안에서는 base.Init()으로 BaseScene의 Init()함수를 불러야 한다.
/// (2) 그리고 해당 SceneType을 현재 Scene으로 변경해주어야 함.
/// </summary>
public class SampleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();                        // (1)

        SceneType = Define.Scene.Game;      // (2)
        
        //Managers.UI.ShowSceneUI<UI_Inven>();

        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
    }
    
    public override void Clear()
    {
        
    }

}
