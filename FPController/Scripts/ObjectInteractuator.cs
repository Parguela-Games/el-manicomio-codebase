﻿using Manicomio.ActionableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectInteractuator : MonoBehaviour {
    [SerializeField] InteractionText interactText;

    [SerializeField] LayerMask interactionMask;

    GameActions actions;
    Camera mainCamera;

    IActionableObject currInteractiveObject;

    private void Awake() {
        actions = new GameActions();
        actions.Player.Interact.performed += OnInteractPerformed;
        PlayerEvents.OnPlayerActivated += () => enabled = true;
        PlayerEvents.OnPlayerDeactivated += () => enabled = false;
    }

    private void OnEnable() {
        actions.Player.Enable();
    }

    private void OnDisable() {
        actions.Player.Disable();
    }

    private void OnDestroy() {
        actions.Player.Interact.performed -= OnInteractPerformed;
        PlayerEvents.OnPlayerActivated -= () => enabled = true;
        PlayerEvents.OnPlayerDeactivated -= () => enabled = false;
    }

    private void Start() {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        CheckInteractions();
    }

    private void CheckInteractions() {
        Ray cameraRay = mainCamera.ViewportPointToRay(Vector3.one / 2f);

        RaycastHit hit;
        IActionableObject objectHit;

        // We are looking towards an interactive object an in range
        if (Physics.Raycast(cameraRay, out hit, 1.5f, interactionMask, QueryTriggerInteraction.Collide)
            && hit.transform.TryGetComponent<IActionableObject>(out objectHit)
            && objectHit.IsInteracterActive()
        ) {
            interactText.SetInteractionText(objectHit.GetInteractionText());
            currInteractiveObject = objectHit;

            interactText.Enable();
        } else if (interactText.enabled) {
            interactText.Disable();
            currInteractiveObject = null;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext obj) {
        currInteractiveObject?.Interact(this.gameObject);
    }

    private void OnDrawGizmos() {
        if (mainCamera == null) return;

        Ray cameraRay = mainCamera.ViewportPointToRay(Vector3.one / 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraRay.origin, cameraRay.direction * 1.5f);
    }

}
