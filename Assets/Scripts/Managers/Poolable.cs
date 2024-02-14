using UnityEngine;

/// <summary>
/// GameObject가 PoolManger에 의해 관리될 수 있도록 하는 Marker 역할의 클래스.
/// </summary>
public class Poolable : MonoBehaviour
{
    /// <summary>
    /// 풀링 상태 표시. <para />
    /// 풀에 반환될 때 => false <para />
    /// 풀에서 꺼내어 사용될 때 => true
    /// </summary>
    public bool isUsing;
}
