using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu]
    public class GameSettings : ScriptableObject
    {
        public float FloatValue;
        public GameObject SomeObjectReference;
    }
}