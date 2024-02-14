using System.Collections.Generic;
using UnityEngine;
using Data;

/// <summary>
/// 읽어들이는 데이터의 포맷 클래스는 ILoader인터페이스를 구현해야 함.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface ILoader<TKey, TValue>
{
    /// <summary>
    /// 데이터를 Dictionary 형태로 변환하는 메서드. 
    /// Dictionary의 키와 값은 인터페이스의 제네릭 타입으로 지정.
    /// </summary>
    /// <returns></returns>
    Dictionary<TKey, TValue> MakeDict();
}

/// <summary>
/// 데이터를 관리하는 매니저.
/// </summary>
public class DataManager
{
    /// <summary>
    /// 게임 내 정보를 담는 Dictionary. 
    /// 처음 게임이 시작될 때, Init() 함수를 통해 필요한 정보를 여기에 캐싱하고,
    /// 이후에 필요할 때는 여기를 참조해 정보를 활용한다.
    /// </summary>
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();
    
    /// <summary>
    /// 게임이 시작할 때 데이터를 로드하는 초기화 메서드. 
    /// LoadJson 메서드를 사용해 지정된 경로의 Json 파일로부터 데이터를 로드하고,
    /// ILoader 인터페이스를 구현하는 객체의 MakeDict 함수를 호출해 정보를 캐싱한다.
    /// </summary>
    public void Init()
    {
        // 다음과 같은 형태로 사용 가능
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
    }

    /// <summary>
    /// Json 파일에서 데이터를 로드하고, 해당 데이터를 처리하기 위한 ILoader 인터페이스를 구현하는 타입의 인스턴스를 생성한다.
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="TLoader"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    TLoader LoadJson<TLoader, TKey, TValue>(string path) where TLoader : ILoader<TKey, TValue>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<TLoader>(textAsset.text);
    }
}
