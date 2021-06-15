using System;
using Assets.Code;
using Assets.Code.BoardData;
using Assets.Code.BoardHelpers;
using Code.Util;
using UnityEngine;

namespace Code
{
    public class Entity : MonoBehaviour
    {       


        [SerializeField] private Vector2Int boardPosition;
        public Vector2Int BoardPosition => boardPosition;

        public MovementDirection Direction { get; private set; }

        [ContextMenu("FitPosition")]
        public void FitPosition()
        {
            boardPosition = new Vector2Int((int)transform.position.x, (int)transform.position.z);
            transform.position = Board.Instance.GetWorldPosition(BoardPosition);
        }

        public void Interact(MovementDirection direction)
        {
            Direction = direction;

            //lock input
            InputCatcher.Instance.IsLocked = true;

            var cell = BoardMovementHelper.GridRaycast(BoardPosition, direction, Board.Instance);

            var currentCell = Board.Instance.GetCell(BoardPosition);
            currentCell.Free();

            if (cell.HasEntity)
            {
                //one step back
                boardPosition = cell.BoardPosition - BoardMovementHelper.MovementDirectionToDirection(direction);

                var targetCell = Board.Instance.GetCell(boardPosition);
                targetCell.ReplaceEntity(this);

                OnInteracting(() => cell.Entity.Interact(direction));
            }
            else
            {
                boardPosition = cell.BoardPosition;
                cell.ReplaceEntity(this);

                OnFalling(() =>
                {
                    //remove from board
                    currentCell = Board.Instance.GetCell(BoardPosition);
                    currentCell.Free();

                    Board.Instance.RemoveEntity(this);



                    //unlock input
                    InputCatcher.Instance.IsLocked = false;
                });
            }
        }

        protected virtual void OnInteracting(Action onComplete) { }
        protected virtual void OnFalling(Action onComplete) { }
    }
}