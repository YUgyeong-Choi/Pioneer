using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Scene에 배치되어 Scene을 구별해주는 GameObject가 가져야할 컴포넌트의 기본 클래스.
/// 새로운 Scene이 추가될 때 마다, 이를 상속하는 클래스를 만들어 GameObject에 부착해야 한다.
/// 이 클래스는 모든 씬에 공통적으로 필요한 기본 설정과 초기화 로직을 제공한다.
/// </summary>
public abstract class BaseScene : MonoBehaviour
{
    // SceneType 프로퍼티는 현재 씬의 유형을 정의. 
    // 이는 각 씬이 어떤 유형의 씬인지 식별하는데 사용됨.
    public Define.Scene SceneType
    {
        get;            // get 접근자는 public으로 설정되어 외부에서 SceneType을 읽을 수 있게 함.
        protected set;  // set 접근자는 protected로 설정되어 상속받은 클래스 내부 또는 자기 자신에서만 SceneType을 설정할 수 있게 함.
    } = Define.Scene.Unknown; // 기본값은 Unknown으로, 씬 상태가 정의되지 않음을 의미.
    
    // Awake 메서드는 MonoBehaviour의 생명주기 중 하나로, 
    // GameObject가 생성될 때 가장 먼저 호출되는 메서드.
    // 이 메서드에서 Init 메서드를 호출하여 씬의 초기화를 진행.
    void Awake()
    {
        Init();
    }

    // Init 메서드는 씬이 초기화될 때 호출되어야 하는 로직을 포함.
    // 기본 구현에서는 EventSystem의 존재를 확인하고, 없을 경우 새로 생성.
    // 이 메서드는 virtual로 선언되어 있어, 상속받는 클래스에서 오버라이드할 수 있으며,
    // 상속받는 클래스의 Init()에서 base.Init()으로 이를 한번 호출함.
    protected virtual void Init()
    {
        var obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)                                                                // EventSystem이 현재 씬에 없다면 새로 생성.
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem"; // EventSystem 프리팹을 인스턴스화하고 이름을 "@EventSystem"으로 설정.
    }

    // Clear 메서드는 씬이 파괴될 때 호출되어야 하는 정리 로직을 정의.
    // 이 메서드는 abstract로 선언되어 있어, 반드시 상속받는 클래스에서 구현해야 함.
    // 씬 전환 시 필요한 리소스 해제, 구독 해제 등의 작업을 여기서 수행할 수 있음.
    public abstract void Clear();
}
