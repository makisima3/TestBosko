using Assets.Code.Units;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Code.BoardHelpers;
using Code;
using Code.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.BoardData
{
    public class Board : MonoBehaviour
    {
        private static Board _instance;

        public static Board Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<Board>();

                return _instance;
            }
        }

        [SerializeField]
        private int boardSize;

        [SerializeField]
        private Cell cellPrefab;

        [SerializeField]
        private Grid grid;

        [SerializeField]
        private bool autoFitPositionsForAll = true;

        private Cell[,] _cells;
        private List<Entity> _cachedEntities;

        public Entity[] CachedEntities
        {
            get => _cachedEntities.ToArray();
        }
        public int BoardSize => boardSize;
        public Cell[,] Cells => _cells;


        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            _cells = new Cell[boardSize, boardSize];
            _cachedEntities = FindEntities().ToList();

            if (autoFitPositionsForAll)
            {
                FitPositionsForAll();
            }

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    var boardPosition = new Vector2Int(x, y);
                    var worldPosition = grid.GetCellCenterWorld(boardPosition.ToV3Int());

                    var cell = Instantiate(cellPrefab.gameObject, worldPosition, Quaternion.identity, transform).GetComponent<Cell>();

                    if (TryGetEntity(boardPosition, out var entity))
                    {
                        cell.Init(boardPosition, entity);
                    }
                    else
                    {
                        cell.Init(boardPosition);
                    }

                    _cells[x, y] = cell;
                }
            }
        }

        public Cell GetCell(Vector2Int boardPosition)
        {
            if (boardPosition.x >= 0 && boardPosition.y >= 0 &&
                boardPosition.x < boardSize && boardPosition.y < boardSize)
                return _cells[boardPosition.x, boardPosition.y];

            return null;
        }

        public Vector3 GetNearestGridPoint(Vector3 position)
        {
            var gridPosition = grid.GetCellCenterWorld(new Vector3Int((int)position.x, (int)position.y, (int)position.z));
            return new Vector3(gridPosition.x, 0f, gridPosition.y);
        }

        public Vector3 GetWorldPosition(Vector2Int gridPosition) => grid.GetCellCenterWorld(gridPosition.ToV3Int());

        public bool TryGetEntity(Vector2Int boardPosition, out Entity entity)
        {
            entity = _cachedEntities.FirstOrDefault(e => e.BoardPosition == boardPosition);
            return entity != null;
        }

        public void RemoveEntity(Entity entity)
        {
            if (entity is Player)
            {
                StartCoroutine(Lose());
            }
            else if(entity is Enemy)
            {
                if(_cachedEntities.Count > 1)
                {
                    _cachedEntities.Remove(entity);
                }
                else
                {
                    StartCoroutine(Victory());
                }
            }
        }

        //Код победы и поражение дублируется, т.к. по тз в обоих случаях лвл перезапускается
        private IEnumerator Lose()
        {
            yield return new WaitForSeconds(3f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator Victory()
        {
            yield return new WaitForSeconds(3f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private Entity[] FindEntities() => FindObjectsOfType<Entity>();

        [ContextMenu("FitPositionsForAll")]
        public void FitPositionsForAll()
        {
            var entities = FindEntities();

            foreach (var entity in entities)
            {
                entity.FitPosition();
            }
        }
    }
}
