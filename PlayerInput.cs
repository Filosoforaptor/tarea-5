using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    private Vector3 inputDirection;
    private bool wasInputActive;

    public Vector3 Axis => inputDirection;

    public event Action OnInputStarted;
    public event Action OnInputStopped;
    public event Action OnFireInput; // Ahora si se re pudrio la momia!

    // Se ejecuta una vez por frame, captura el input del jugador y dispara eventos según corresponda
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        inputDirection = new Vector3(inputX, inputY, 0f);

        bool isInputActive = (inputX != 0f || inputY != 0f);

        // Detecta cuando el jugador comienza a moverse
        if (isInputActive && !wasInputActive)
        {
            OnInputStarted?.Invoke();
        }
        // Detecta cuando el jugador deja de moverse
        else if (!isInputActive && wasInputActive)
        {
            OnInputStopped?.Invoke();
        }

        wasInputActive = isInputActive;

        // Detecta el input de disparo
        if (Input.GetButtonDown("Fire1")) // "Fire1" normalmente está mapeado a Ctrl Izq, click izquierdo del mouse
        {
            Debug.LogWarning("¡Input de disparo (Fire1) detectado!");
            OnFireInput?.Invoke();
        }
    }
}