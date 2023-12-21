using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // El multiplicador de parallax controla la velocidad a la que el fondo se desplaza en relaci�n con la c�mara.
    [SerializeField] private float parallaxMultiplier;

    // Transform del objeto de la c�mara principal.
    private Transform cameraTransform;
    // Posici�n anterior de la c�mara. Se utiliza para calcular la cantidad de desplazamiento.
    private Vector3 previousCameraPosition;
    // Ancho del sprite para determinar cu�ndo reiniciar la posici�n del fondo.
    private float spriteWidth;
    // Posici�n inicial del fondo.
    private float startPosition;

    void Start()
    {
        // Obtiene el transform de la c�mara principal.
        cameraTransform = Camera.main.transform;
        // Guarda la posici�n inicial de la c�mara.
        previousCameraPosition = cameraTransform.position;
        // Obtiene el ancho del sprite del componente SpriteRenderer.
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        // Guarda la posici�n inicial del fondo.
        startPosition = transform.position.x;
    }

    void Update()
    {
        // Calcula el desplazamiento de la c�mara desde el frame anterior y lo multiplica por el multiplicador de parallax.
        float deltaCameraX = (cameraTransform.position.x - previousCameraPosition.x) * parallaxMultiplier;
        // Calcula la cantidad total que se debe mover el fondo.
        float moveAmount = cameraTransform.position.x * (1 - parallaxMultiplier);

        // Mueve el fondo de acuerdo con el desplazamiento calculado.
        transform.Translate(new Vector3(deltaCameraX, 0, 0));
        // Actualiza la posici�n anterior de la c�mara para el pr�ximo frame.
        previousCameraPosition = cameraTransform.position;

        // Si el fondo se ha movido m�s all� de su posici�n inicial m�s el ancho del sprite, actualiza su posici�n.
        if (moveAmount > startPosition + spriteWidth)
        {
            transform.Translate(new Vector3(spriteWidth, 0, 0));
            startPosition += spriteWidth;
        }
        // Si el fondo se ha movido m�s all� de su posici�n inicial menos el ancho del sprite, actualiza su posici�n en la direcci�n opuesta.
        else if (moveAmount < startPosition - spriteWidth)
        {
            transform.Translate(new Vector3(-spriteWidth, 0, 0));
            startPosition -= spriteWidth;
        }
    }
}
