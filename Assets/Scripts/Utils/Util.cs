using UnityEngine;

/// <summary>
/// 기능성 함수들을 담는 클래스
/// </summary>
public class Util
{
    /// <summary>
    /// GameObject에게서 원하는 컴포넌트를 가져온다.
    /// 없다면, 새로 만들어서 붙이고 가져온다.
    /// </summary>
    /// <param name="go">컴포넌트를 가져올 GameObject</param>
    /// <typeparam name="T">컴포넌트의 타입</typeparam>
    /// <returns>컴포넌트</returns>
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    } 

    /// <summary>
    /// 부모 GameObject의 자식 중에서 오브젝트를 찾아 반환하는 함수.
    /// </summary>
    /// <param name="go">검색을 시작하는 부모 GameObject</param>
    /// <param name="name">찾고자하는 component의 이름 (선택, 기본값 null)</param>
    /// <param name="recursive">자식의 자식까지도 찾을 것인지 (선택, 기본값 false)</param>
    /// <typeparam name="T">찾고자하는 컴포넌트의 타입</typeparam>
    /// <returns>name을 입력하지 않으면, 찾고자 하는 컴포넌트 중 가장 먼저 찾아지는 컴포넌트를 반환. 찾을 수 없다면 null</returns>
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;
        if (recursive)      // 재귀 O (= 자식의 자식까지도 찾는다)
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    // 이름이 비어있거나 찾던 이름과 동일하면
                    return component;
            }
        }
        else        // 재귀 X (= 직속 자식에 한해서만 찾는 경우)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                // 이름이 비어있거나 찾던 이름과 동일하면
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// 부모 GameObject의 자식 중에서 GameObject를 찾아 반환하는 함수.
    /// </summary>
    /// <param name="go">부모 GameObject</param>
    /// <param name="name">찾고자 하는 GameObject의 이름 (선택, 기본값 null)</param>
    /// <param name="recursive">자식의 자식까지도 찾을 것인지 (선택, 기본값 false)</param>
    /// <returns></returns>
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {   
        // 모든 GameObject는 Transform을 갖기 때문에
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        return transform.gameObject;
    }
    
}
