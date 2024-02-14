using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Scene에 배치되어 Scene을 구별해주는 GameObject가 가져야할 컴포넌트의 기본 클래스.
/// 새로운 Scene이 추가될 때 마다, 이를 상속하는 클래스를 만들어 GameObject에 부착해야 한다.
/// </summary>
public abstract class BaseScene : MonoBehaviour
{
    // Scene
    public Define.Scene SceneType
    {
        get;            // get은 public
        protected set;  // set은 protected
    } = Define.Scene.Unknown;
    
    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        var obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();
}
