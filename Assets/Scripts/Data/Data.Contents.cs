using System;
using System.Collections.Generic;

// 데이터를 읽고 저장하는 포맷의 샘플

namespace Data
{
    // 캐릭터의 데이터를 나타내는 클래스.
    [Serializable]
    public class Stat
    {
        public int level;
        public int hp;
        public int attack;
    }

    /// <summary>
    /// ILoader 인터페이스를 구현하는 클래스.
    /// Json 파일로부터 로드된 Stat 객체의 리스트를 Dictionary 형태로 변환하는 MakeDict 메서드를 포함.
    /// </summary>
    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();
        
        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            foreach (var item in stats)
                dict.Add(item.level, item);

            return dict;
        }
    }
}

