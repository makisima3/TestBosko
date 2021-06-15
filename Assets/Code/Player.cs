using System;
using Assets.Code.BoardData;
using Assets.Code.BoardHelpers;
using Code;
using DG.Tweening;
using UnityEngine;

namespace Assets.Code
{
    class Player : Entity
    {
        private static Player _instance;

        public static Player Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<Player>();

                return _instance;
            }
        }

        [SerializeField] private GameObject[] bones;
        [SerializeField] private Rigidbody middleSpine;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform characterMesh;

        [SerializeField] private float fallForce = 5f;
        [SerializeField] private float moveSpeed = 3f;

        private void Start()
        {
            InputCatcher.Instance.OnMove.AddListener(Interact);
        }

        protected override void OnInteracting(Action onComplete)
        {
            var targetPosition = Board.Instance.GetWorldPosition(BoardPosition);
            var distance = Vector3.Distance(transform.position, targetPosition);

            ViewPush();

            //invers rotation only for player
            //if (Direction == MovementDirection.Down || Direction == MovementDirection.Up)
            //    transform.rotation = BoardMovementHelper.GetRotation(Direction) * Quaternion.Euler(Vector3.up * 180);
            //else
            //    transform.rotation = BoardMovementHelper.GetRotation(Direction) * Quaternion.Euler(Vector3.up * 180);

            transform.rotation = BoardMovementHelper.GetRotation(Direction) * Quaternion.Euler(Vector3.up);

            transform.DOMove(targetPosition, distance / moveSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() => { onComplete?.Invoke(); ViewStop(); });

        }

        protected override void OnFalling(Action onComplete)
        {
            var targetPosition = Board.Instance.GetWorldPosition(BoardPosition);
            var distance = Vector3.Distance(transform.position, targetPosition);

            ViewPush();

            //invers rotation only for player
            transform.rotation = BoardMovementHelper.GetRotation(Direction) * Quaternion.Euler(Vector3.up);// * Quaternion.Euler(Vector3.up * 180);
            transform.DOMove(targetPosition, distance / moveSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    /*TODO: enable falling ragdoll*/
                    EnableRagdol(bones, middleSpine, Direction);
                    onComplete?.Invoke();
                });
        }

        private void ViewPush()
        {
            characterMesh.rotation = Quaternion.Euler(Vector3.up * 90f);
            animator.SetTrigger("push");
        }

        private void ViewStop()
        {
            animator.SetTrigger("idle");
            transform.rotation = Quaternion.identity;
            characterMesh.rotation = Quaternion.identity;
        }

        private void EnableRagdol(GameObject[] bones, Rigidbody middleSpine, MovementDirection direction)
        {
            animator.enabled = false;

            foreach (var bone in bones)
            {
                bone.GetComponent<Rigidbody>().isKinematic = false;
                bone.GetComponent<Collider>().enabled = true;
            }

            var force = BoardMovementHelper.MovementDirectionToDirection(direction);

            middleSpine.AddForce(new Vector3(force.x, 0f, force.y) * fallForce, ForceMode.Impulse);
        }
    }
}
