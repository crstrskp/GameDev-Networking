using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealthDisplay : MonoBehaviourPun
{
    [Tooltip("UI Text to display Player's Name")]
    [SerializeField] private Text playerNameText;

    [Tooltip("Pixel offset from the player target")]
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    
    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField] private Slider playerHealthSlider;


    private PlayerHandler m_playerHandler;
    private Health m_health;

    private PlayerHandler target; 
    private Transform targetTransform;
    private Renderer targetRenderer;
    private CanvasGroup _canvasGroup;
    private float characterControllerHeight;
    private Vector3 targetPosition;

    void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    void OnDestroy() => m_health.HealthChanged -= UpdateHealth;

    private void UpdateHealth(int currentHealth, int maxHealth)
    {
        playerHealthSlider.minValue = 0;
        playerHealthSlider.maxValue = maxHealth;
        playerHealthSlider.value = currentHealth;
    }

    /// <summary>
    /// MonoBehaviour method called after all Update functions have been called. This is useful to order script execution.
    /// In our case since we are following a moving GameObject, we need to proceed after the player was moved during a particular frame.
    /// </summary>
    void LateUpdate () 
    {
        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer!=null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }
        
        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform!=null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            
            this.transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
        }
    }

   	/// <summary>
    /// Assigns a Player Target to Follow and represent.
    /// </summary>
    /// <param name="target">Target.</param>
    public void SetTarget(PlayerHandler _target)
    {
        if (_target == null) {
            Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency because we are going to reuse them.
        this.target = _target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponentInChildren<Renderer>();


        CharacterController _characterController = this.target.GetComponent<CharacterController> ();

        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null){
            characterControllerHeight = _characterController.height;
        }

        if (playerNameText != null) {
            playerNameText.text = this.target.photonView.Owner.NickName;
        }
    }
}
