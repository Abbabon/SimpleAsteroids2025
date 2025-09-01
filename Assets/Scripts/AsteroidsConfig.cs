using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AsteroidsConfig", menuName = "ScriptableObjects/AsteroidsConfig", order = 1)]
public class AsteroidsConfig : ScriptableObject
{
        public List<AsteroidData> asteroidDatas;

        public AsteroidData GetConfigForLevel(int level)
        {
                return asteroidDatas.Find(data => data.Level == level);
        }
}