using UnityEngine;

/// <summary>
/// Resource를 관리하는 매니저.
/// 리소스 로딩, 인스턴스화, 파괴를 관리.
/// </summary>
public class ResourceManger 
{
    /// <summary>
    /// 지정된 경로에서 T 타입의 리소스를 로드하는 함수.
    /// 로드하려는 리소스가 GameObject이고, Poolable이 있는 경우에는 풀링을 적용.
    /// </summary>
    /// <param name="path">경로</param>
    /// <typeparam name="T">로드하려는 형태</typeparam>
    /// <returns></returns>
    public T Load<T>(string path) where T : Object
    {
        // Load하려는 오브젝트가 GameObject고, PoolManager가 관리하는 _pool(Dict)에 있는 오리지널 중 하나라면,
        // 그 녀석을 가져오고
        // 그렇지 않다면 새로 Load한다.
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        
        return Resources.Load<T>(path);
    }

    /// <summary>
    /// 지정된 경로의 프리팹을 인스턴스화하는 함수.
    /// </summary>
    /// <param name="path">Prefabs/ 아래의 경로</param>
    /// <param name="parent">로드된 인스턴스의 부모 (선택)</param>
    /// <returns>GameObject 인스턴스</returns>
    public GameObject Instantiate(string path, Transform parent = null)
    {
        // 1. original 이미 들고있으면 바로 사용.
        var original = Load<GameObject>($"Prefabs/{path}");       // 원본
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        // 2. 만약에 풀링 오브젝트라면, Poop에서 가져온다.
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;
        
        var go = Object.Instantiate(original, parent);        // 복사본
        go.name = original.name;
        
        return go;
    }

    /// <summary>
    /// 게임 오브젝트 파괴.
    /// 오브젝트에 Poolable 오브젝트가 있으면, Pool로 반환.
    /// </summary>
    /// <param name="go"></param>
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;
        
        // 만약에 풀링 오브젝트라면, Destroy하지않고 Pool로 복귀시킨다.
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        
        Object.Destroy(go);
    }
}
