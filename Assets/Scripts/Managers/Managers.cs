using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 모든 기능별 매니저들을 중앙에서 관리하는 싱글톤 구현체.
/// </summary>
[DefaultExecutionOrder(0)]
public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일한 인스턴스를 담을 변수.
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 인스턴스를 참조하는 property

    // 기능별 매니저 인스턴스들
    // 새로운 Manager가 추가될 때, 아래에 하나씩 추가.
    private DataManager _data = new DataManager();
    private InputManager _input = new InputManager();
    private PoolManager _pool = new PoolManager();
    private ResourceManger _resource = new ResourceManger();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();

    private FusionManager _fusion = new FusionManager();

    public static DataManager Data => Instance._data;
    public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManger Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => s_instance._ui;
    public static FusionManager Fusion => s_instance._fusion;
    
    void Start()
    {
        Init();
    }

    // private void Update()
    // {
    //     //_input.OnUpdate();
    // }

    /// <summary>
    /// Managers 클래스와 모든 기능별 매니저 컴포넌트의 초기화를 담당.
    /// </summary>
    static void Init()
    {
        // s_instance가 null일 때만 Managers를 찾아 Instance에 할당
        if (s_instance != null) return;
        
        GameObject go = GameObject.Find("@Managers");
        if (go == null)
        {
            go = new GameObject {name = "@Managers"};
            go.AddComponent<Managers>();
        }
        
        DontDestroyOnLoad(go);
        s_instance = go.GetComponent<Managers>();
        
        // 여기 안에서는 Instance를 호출하면 Infinite Loop에 걸리기 때문에, 호출 금지.
        
        s_instance._data.Init();
        s_instance._pool.Init();
        s_instance._sound.Init();

        s_instance._fusion.Init();
    }

    /// <summary>
    /// Scene전환 시, 필요한 리소스 정리를 수행하는 함수.
    /// </summary>
    public static void Clear()
    {   
        // DataManager는 Clear X
        Sound.Clear();
        //Input.Clear();
        //UI.Clear();
        Scene.Clear();
        
        Pool.Clear();       // 의도적으로 마지막에 Clear. 왜? 다른 Manager에서 pool 오브젝트를 사용할 수 있기 때문.
    }
}

