using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScrpt : MonoBehaviour
{
    public Transform target; // Referencja do obiektu gracza, za kt�rym ma pod��a� kamera
    private Vector3 offset; // Przechowuje pocz�tkowy offset mi�dzy kamer� a obiektem gracza

    void Start()
    {
        offset = transform.position - target.position; // Obliczanie pocz�tkowego offsetu
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset; // Obliczanie po��danej pozycji kamery
        transform.position = desiredPosition; // Przypisywanie po��danej pozycji do kamery
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
