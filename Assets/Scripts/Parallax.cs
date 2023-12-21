using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // El multiplicador de parallax controla la velocidad a la que el fondo se desplaza en relación con la cámara.
    [SerializeField] private float parallaxMultiplier;

    // Transform del objeto de la cámara principal.
    private Transform cameraTransform;
    // Posición anterior de la cámara. Se utiliza para calcular la cantidad de desplazamiento.
    private Vector3 previousCameraPosition;
    // Ancho del sprite para determinar cuándo reiniciar la posición del fondo.
    private float spriteWidth;
    // Posición inicial del fondo.
    private float startPosition;

    void Start()
    {
        // Obtiene el transform de la cámara principal.
        cameraTransform = Camera.main.transform;
        // Guarda la posición inicial de la cámara.
        previousCameraPosition = cameraTransform.position;
        // Obtiene el ancho del sprite del componente SpriteRenderer.
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        // Guarda la posición inicial del fondo.
        startPosition = transform.position.x;
    }

    void Update()
    {
        // Calcula el desplazamiento de la cámara desde el frame anterior y lo multiplica por el multiplicador de parallax.
        float deltaCameraX = (cameraTransform.position.x - previousCameraPosition.x) * parallaxMultiplier;
        // Calcula la cantidad total que se debe mover el fondo.
        float moveAmount = cameraTransform.position.x * (1 - parallaxMultiplier);

        // Mueve el fondo de acuerdo con el desplazamiento calculado.
        transform.Translate(new Vector3(deltaCameraX, 0, 0));
        // Actualiza la posición anterior de la cámara para el próximo frame.
        previousCameraPosition = cameraTransform.position;

        // Si el fondo se ha movido más allá de su posición inicial más el ancho del sprite, actualiza su posición.
        if (moveAmount > startPosition + spriteWidth)
        {
            transform.Translate(new Vector3(spriteWidth, 0, 0));
            startPosition += spriteWidth;
        }
        // Si el fondo se ha movido más allá de su posición inicial menos el ancho del sprite, actualiza su posición en la dirección opuesta.
        else if (moveAmount < startPosition - spriteWidth)
        {
            transform.Translate(new Vector3(-spriteWidth, 0, 0));
            startPosition -= spriteWidth;
        }
    }
}
