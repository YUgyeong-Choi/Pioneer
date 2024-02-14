using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링을 담당하는 매니저. 
/// 내부에 Pool을 갖고 이를 활용해 오브젝트 풀링을 구현한다.
/// </summary>
public class PoolManager
{
    /// <summary>
    /// 특정 게임 오브젝트의 종류에 따른 오브젝트 풀을 구현. <para />
    /// 각 풀 마다 Original GameObject,
    /// 풀에 속한 오브젝트들의 Root 컨테이너, 
    /// 재사용을 위해 비활성화된 오브젝트들을 저장하는 _poolStack을 갖는다. 
    /// </summary>
    class Pool
    {
        /// <summary>
        /// 오리지널 GameObject
        /// </summary>
        public GameObject Original { get; private set; }
        /// <summary>
        /// 풀에 속한 오브젝트들의 Root 컨테이너
        /// </summary>
        public Transform Root { get; private set; }
        
        /// <summary>
        /// 재사용을 위해 비활성화된 오브젝트들을 저장하는 스택
        /// </summary>
        private Stack<Poolable> _poolStack = new Stack<Poolable>();

        /// <summary>
        /// 제공된 오리지널 게임오브젝트와 지정된 크기를 기반으로 Pool을 초기화하는 함수.
        /// </summary>
        /// <param name="original">오리지널 게임오브젝트</param>
        /// <param name="count">Pool 크기 지정</param>
        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < count; i++)
                Push(Create());
        }

        /// <summary>
        /// Original 게임 오브젝트를 복제하여 새로운 Poolable오브젝트를 생성한다.
        /// </summary>
        /// <returns></returns>
        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;

            return go.GetOrAddComponent<Poolable>();
        }

        /// <summary>
        /// 사용이 끝난 Poolable 오브젝트를 풀에 반환한다.
        /// 반환된 오브젝트는 비활성화된고, 풀의 루트 컨테이너로 이동된다.
        /// </summary>
        /// <param name="poolable"></param>
        public void Push(Poolable poolable)
        {
            if (poolable == null) return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.isUsing = false;
            
            _poolStack.Push(poolable);
        }

        /// <summary>
        /// 풀에서 하나의 Poolable 오브젝트를 꺼내 활성화하고 반환한다.
        /// </summary>
        /// <param name="parent">풀에서 꺼낸 오브젝트의 부모 (선택, 기본값 null)</param>
        /// <returns></returns>
        public Poolable Pop(Transform parent = null)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            poolable.gameObject.SetActive(true);

            // Dont destroy on load 해제 용도
            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;
            
            poolable.transform.parent = parent;
            poolable.isUsing = true;
            
            return poolable;
        }
    }

    /// <summary>
    /// 풀들을 담는 Dictionary. 키는 Original GameObject의 name, 값은 Pool
    /// </summary>
    private Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    /// <summary>
    /// 풀에 속한 모든 GameObject의 부모
    /// </summary>
    private Transform _root;
    
    /// <summary>
    /// GameObject Pool들의 Root 컨테이너를 초기화하고, DontDestroyOnLoad로 Scene 전환시에도 유지되도록.
    /// </summary>
    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    /// <summary>
    /// 지정된 Original GameObject와 초기 Pool 크기를 기반으로 새로운 Pool을 생성하고,
    /// 관리할 수 있도록 Dictionary에 추가.
    /// </summary>
    /// <param name="original">Original GameObject</param>
    /// <param name="count">Pool의 크기</param>
    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root;
        
        _pool.Add(original.name, pool);
    }
    
    /// <summary>
    /// 사용이 끝난 Poolable 오브젝트를 Pool에 집어넣는 함수.
    /// </summary>
    /// <param name="poolable">사용이 끝난 오브젝트의 Poolable 컴포넌트</param>
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }
        
        _pool[name].Push(poolable);
    }

    /// <summary>
    /// 지정된 Original GameObject에 해당하는 풀에서 Poolable 오브젝트를 꺼내 활성화하여 반환.
    /// </summary>
    /// <param name="original">Original GameObject</param>
    /// <param name="parent">활성화된 GameObject의 부모 (선택, 기본값 null)</param>
    /// <returns>활성화된 GameObject의 Poolable</returns>
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);
        
        return _pool[original.name].Pop(parent);
    }

    /// <summary>
    /// 지정된 이름의 Original GameObject를 찾아 반환.
    /// </summary>
    /// <param name="name">찾고자 하는 Original GameObject의 이름</param>
    /// <returns></returns>
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        
        return _pool[name].Original;
    }

    /// <summary>
    /// 모든 풀을 정리하고, Root 컨테이너 내의 모든 GameObject를 파괴한 뒤, 관리 Dictionary를 비운다.
    /// </summary>
    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);
        
        _pool.Clear();
    }
}
