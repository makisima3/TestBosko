using System;
using Assets.Code.BoardData;
using System.Collections.Generic;
using Code;
using Code.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code.BoardHelpers
{
    public static class BoardMovementHelper
    {
        private static readonly Dictionary<MovementDirection, Vector2Int> MovementDirectionToWorldDirection = new Dictionary<MovementDirection, Vector2Int>
        { 
            { MovementDirection.Up,     Vector2Int.up },
            { MovementDirection.Down,   Vector2Int.down },
            { MovementDirection.Left,   Vector2Int.left },
            { MovementDirection.Right,  Vector2Int.right }
        };
        
        private static readonly Dictionary<MovementDirection, float> MovementDirectionToRotation = new Dictionary<MovementDirection, float>
        { 
            { MovementDirection.Up,     0f },//0 
            { MovementDirection.Down,   180f },//180 
            { MovementDirection.Left,   270f },
            { MovementDirection.Right,  90f }//90
        };

        public static Cell GridRaycast(Vector2Int origin, MovementDirection direction, Board board)
        {
            var dir = MovementDirectionToDirection(direction);
            var position = origin + dir;
            
            var loopProtection = board.BoardSize+1;
            while (loopProtection > 0)
            {
                var cell = board.GetCell(position);
                if (cell == null)
                    return board.GetCell(position - dir);// return last cell on board

                if (cell.HasEntity)
                    return cell;

                position += dir;
                loopProtection--;
            }

            throw new Exception("Endless loop");
        }

        public static Quaternion GetRotation(MovementDirection direction)
        {
            return Quaternion.Euler(Vector3.up * MovementDirectionToRotation[direction]);
        }
        
        public static MovementDirection DirectionToMovementDirection(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x > 0 ? MovementDirection.Right : MovementDirection.Left;
            }
            else
            {
                return direction.y > 0 ? MovementDirection.Up : MovementDirection.Down;
            }
        }
        
        public static Vector2Int MovementDirectionToDirection(MovementDirection direction)
        {
            return MovementDirectionToWorldDirection[direction];
        }
    }
}
