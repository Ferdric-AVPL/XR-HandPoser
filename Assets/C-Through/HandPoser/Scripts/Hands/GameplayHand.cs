using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace CThrough.XR.Hands.Poser
{
    public class GameplayHand : BaseHand
    {
        // The interactor we react to
        [SerializeField] private XRBaseInteractor targetInteractor = null;

        //  Animator components
        private Animator animator = null;
        private HandAnimator handAnimator = null;

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
            handAnimator = GetComponent<HandAnimator>();
        }

        private void OnEnable()
        {
            // Subscribe to selected events

            targetInteractor.onSelectEntered.AddListener(DisableAnimator);
            targetInteractor.onSelectEntered.AddListener(TryApplyObjectPose);

            targetInteractor.onSelectExited.AddListener(EnableAnimator);
            targetInteractor.onSelectExited.AddListener(TryApplyDefaultPose);
        }

        private void OnDisable()
        {
            //Unsubscribe to selected events

            targetInteractor.onSelectEntered.RemoveListener(DisableAnimator);
            targetInteractor.onSelectEntered.RemoveListener(TryApplyObjectPose);

            targetInteractor.onSelectExited.RemoveListener(EnableAnimator);
            targetInteractor.onSelectExited.RemoveListener(TryApplyDefaultPose);
        }

        private void TryApplyObjectPose(XRBaseInteractable interactable)
        {
            // Try and get pose container, and apply
            if (interactable.TryGetComponent(out PoseContainer poseContainer))
                ApplyPose(poseContainer.pose);
        }

        private void TryApplyDefaultPose(XRBaseInteractable interactable)
        {
            // Try and get pose container, and apply
            if (interactable.TryGetComponent(out PoseContainer poseContainer))
                ApplyDefaultPose();
        }

        private void DisableAnimator(XRBaseInteractable interactable)
        {
            if (handAnimator != null)
                handAnimator.enabled = false;

            if (animator != null)
                animator.enabled = false;
        }

        private void EnableAnimator(XRBaseInteractable interactable)
        {
            if (animator != null)
                animator.enabled = true;

            if (handAnimator != null)
                handAnimator.enabled = true;
        }/**/

        public override void ApplyOffset(Vector3 position, Quaternion rotation)
        {
            // Invert since the we're moving the attach point instead of the hand
            Vector3 finalPosition = position * -1.0f;
            Quaternion finalRotation = Quaternion.Inverse(rotation);

            // Since it's a local position, we can just rotate around zero
            finalPosition = finalPosition.RotatePointAroundPivot(Vector3.zero, finalRotation.eulerAngles);

            // Set the position and rotach of attach
            targetInteractor.attachTransform.localPosition = finalPosition;
            targetInteractor.attachTransform.localRotation = finalRotation;
        }

        private void OnValidate()
        {
            // Let's have this done automatically, but not hide the requirement
            if (!targetInteractor)
                targetInteractor = GetComponentInParent<XRBaseInteractor>();
        }
    }
}