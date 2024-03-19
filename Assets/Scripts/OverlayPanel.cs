using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayPanel : UIPanel
{
    public bool showPrompt;
    bool showPetPrompt;
    bool showCallPrompt;
    bool showPetLocation;
    bool showCompanionTarget;
    Vector3 locationPosition;
    GameObject interactTarget;
    GameObject petTarget;
    GameObject playerTarget;
    GameObject companionTarget;


    [SerializeField] float companionTargetYOffset;


    [SerializeField] RectTransform petPrompt;
    [SerializeField] RectTransform petLocation;
    [SerializeField] float petPromptOffset;

    [SerializeField] RectTransform interactOverlayPrompt;
    [SerializeField] float interactOverlayYOffset;
    [SerializeField] RectTransform canvasRect;

    [SerializeField] RectTransform callPrompt;
    [SerializeField] float callPromptYOffset;
    public override void Activate()
    {
        base.Activate();
        DisablePetOverlay();
        DisablePrompt();
        DisablePetLocation();
        DisableCallOverlay();
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override void Setup()
    {
        base.Setup();
    }

    public void EnableCompanionTarget()
    {
        companionTarget = CompanionController.Instance.targetToAttack;
        showCompanionTarget = true;
        showPetLocation = false;
        petLocation.gameObject.GetComponent<Image>().color = UIManager.Instance.targetColor;
        petLocation.gameObject.SetActive(true);

    }

    public void DisableCompanionTarget()
    {
        showCompanionTarget = false;
        petLocation.gameObject.SetActive(false);
    }
    public void EnableCallOverlay()
    {
        playerTarget = PlayerController.Instance.gameObject;
        showCallPrompt = true;
        callPrompt.gameObject.SetActive(true);
        callPrompt.gameObject.GetComponent<Image>().color = UIManager.Instance.callColor;
        Invoke("DisableCallOverlay", 1f);
    }

    public void DisableCallOverlay()
    {
        showCallPrompt = false;
        callPrompt.gameObject.SetActive(false);
    }


    public void EnablePetOverlay()
    {
        petTarget = CompanionController.Instance.gameObject;
        float petOffsetY = petTarget.transform.position.y + petPromptOffset;
        Vector3 offsetPos = new Vector3(petTarget.transform.position.x, petOffsetY, petTarget.transform.position.z);

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

        // Set
        petPrompt.localPosition = canvasPos;
        showPetPrompt = true;
        petPrompt.gameObject.SetActive(true);
    }

    public void EnablePetLocation()
    {
        locationPosition = CompanionController.Instance.gameObject.transform.position;//Camera.main.ScreenToWorldPoint(Input.mousePosition);
        petLocation.gameObject.SetActive(true);
        petLocation.gameObject.GetComponent<Image>().color = UIManager.Instance.locationColor;
        showCompanionTarget = false;
        showPetLocation = true;
      
    }

    public void DisablePetLocation()
    {
        showPetLocation = false;
        petLocation.gameObject.SetActive(false);
    }

    public void DisablePetOverlay()
    {
        showPetPrompt = false;
       
        petPrompt.gameObject.SetActive(false);
    }

    public void EnableOverlayPrompt(GameObject _target, float _offset)
    {
        interactTarget = _target;
        interactOverlayYOffset = _offset;

        showPrompt = true;
        interactOverlayPrompt.gameObject.SetActive(true);
    }

    public void DisablePrompt()
    {
        showPrompt = false;
        interactOverlayPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (showPetLocation)
        {
          
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(locationPosition);

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

            // Set
            petLocation.localPosition = canvasPos;
        }

        if (showCompanionTarget)
        {
            if (companionTarget == null)
            {
                DisableCompanionTarget();
            }
            float petOffsetY = companionTarget.transform.position.y + companionTargetYOffset;
            Vector3 offsetPos = new Vector3(companionTarget.transform.position.x, petOffsetY, companionTarget.transform.position.z);

            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

            // Set
            petLocation.localPosition = canvasPos;
        }

        if (showPetPrompt)
        {
            float petOffsetY = petTarget.transform.position.y + petPromptOffset;
            Vector3 offsetPos = new Vector3(petTarget.transform.position.x, petOffsetY, petTarget.transform.position.z);

            // Calculate *screen* position (note, not a canvas/recttransform position)
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

            // Set
            petPrompt.localPosition = canvasPos;
        }

        if (showCallPrompt)
        {
            float callOffsetY = playerTarget.transform.position.y + callPromptYOffset;
            Vector3 offsetPos = new Vector3(playerTarget.transform.position.x, callOffsetY, playerTarget.transform.position.z);

            // Calculate *screen* position (note, not a canvas/recttransform position)
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

            // Set
            callPrompt.localPosition = canvasPos;
        }

        if (showPrompt)
        {
            if (interactTarget == null)
            {
                DisablePrompt();
            }
            float offsetPosY = interactTarget.transform.position.y + interactOverlayYOffset;

            // Final position of marker above GO in world space
            Vector3 offsetPos = new Vector3(interactTarget.transform.position.x, offsetPosY, interactTarget.transform.position.z);

            // Calculate *screen* position (note, not a canvas/recttransform position)
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

            // Set
            interactOverlayPrompt.localPosition = canvasPos;
        }
     
    }
}
