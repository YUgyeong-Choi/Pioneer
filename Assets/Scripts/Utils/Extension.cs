using UnityEngine;

/// <summary>
/// 확장 기능을 담는 클래스
/// </summary>
public static class Extension
{
    /// <summary>
    /// GameObject에게서 원하는 컴포넌트를 가져온다.
    /// 없다면, 새로 만들어서 붙이고 가져온다.
    /// 내부적으로 Util.GetOrAddComponent를 사용한다.
    /// </summary>
    /// <param name="go">입력 X</param>
    /// <typeparam name="T">컴포넌트의 타입</typeparam>
    /// <returns>컴포넌트</returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    } 
}
