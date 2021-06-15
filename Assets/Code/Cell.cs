using Code;
using UnityEngine;

namespace Assets.Code.BoardData
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Entity entity;

        public Entity Entity
        {
            get => entity;
            set => entity = value;
        }

        public bool HasEntity => Entity != null;

        public Vector2Int BoardPosition { get; private set; }

        public void Init(Vector2Int boardPosition, Entity entity=null)
        {
            Entity = entity;
            if (Entity != null)
            {
                Entity.transform.SetParent(transform);
            }

            BoardPosition = boardPosition;
        }

        public void ReplaceEntity(Entity entity)
        {
            Entity = entity;
            Entity.transform.SetParent(transform);
        }
        
        public void Free()
        {
            Entity = null;
        }
    }
}
