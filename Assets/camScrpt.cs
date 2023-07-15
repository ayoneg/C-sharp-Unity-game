using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScrpt : MonoBehaviour
{
    public Transform target; // Referencja do obiektu gracza, za którym ma pod¹¿aæ kamera
    private Vector3 offset; // Przechowuje pocz¹tkowy offset miêdzy kamer¹ a obiektem gracza

    void Start()
    {
        offset = transform.position - target.position; // Obliczanie pocz¹tkowego offsetu
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset; // Obliczanie po¿¹danej pozycji kamery
        transform.position = desiredPosition; // Przypisywanie po¿¹danej pozycji do kamery
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
