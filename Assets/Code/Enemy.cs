using System;
using Assets.Code.BoardData;
using Assets.Code.BoardHelpers;
using Code;
using DG.Tweening;
using UnityEngine;

namespace Assets.Code.Units
{
    public class Enemy : Entity
    {
        [SerializeField] private GameObject[] bones;
        [SerializeField] private Rigidbody middleSpine;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform characterMesh;

        [SerializeField] private float fallForce = 5f;
        [SerializeField] private float moveSpeed = 3f;

        protected override void OnInteracting(Action onComplete)
        {
            var targetPosition = Board.Instance.GetWorldPosition(BoardPosition);
            var distance = Vector3.Distance(transform.position, targetPosition);

            FlyView();

            transform.rotation = BoardMovementHelper.GetRotation(Direction);
            transform.DOMove(targetPosition, distance / moveSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() => { onComplete?.Invoke(); ViewStop(); });

        }

        protected override void OnFalling(Action onComplete)
        {
            var targetPosition = Board.Instance.GetWorldPosition(BoardPosition);
            var distance = Vector3.Distance(transform.position, targetPosition);
            
            FlyView();

            transform.rotation = BoardMovementHelper.GetRotation(Direction);
            transform.DOMove(targetPosition, distance / moveSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    /*TODO: enable falling ragdoll*/
                    EnableRagdol(bones, middleSpine, Direction);
                    onComplete?.Invoke();
                });

        }

        private void FlyView()
        {
            var dir = BoardMovementHelper.MovementDirectionToDirection(Direction);

            characterMesh.rotation = Quaternion.Euler(new Vector3(0f,0f ,180f));
            characterMesh.position = new Vector3(characterMesh.position.x, 1.3f, characterMesh.position.z);
            animator.SetTrigger("fly");
        }

        private void ViewStop()
        {
            animator.SetTrigger("idle");
            characterMesh.rotation = Quaternion.identity;
            characterMesh.position = new Vector3(0f, -0.5f, 0f);
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
