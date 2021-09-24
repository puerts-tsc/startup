using UnityEngine;

/*
 * Prefabs that will need to be used during the game by a lot of objects
 */
namespace Dungeon.PrefabManager
{
    public class ObjectPrefabs : MonoBehaviour {

        //On object dead, create this GameObject (Some particle effect)
        public UnityEngine.GameObject OnObjectDeath;
    
        //On attack is missed (Some particle)
        public UnityEngine.GameObject OnAttackMiss;
    
        //On attack is hit (Some particcle)
        public UnityEngine.GameObject OnAttackHit;

        //On Level Up (Some particle)
        public UnityEngine.GameObject OnLeveup;

        //Instance to this script
        public static ObjectPrefabs instance;

        // Use this for initialization
        void Awake() {
            instance = this;
        }
	
    }
}
