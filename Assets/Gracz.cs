using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gracz : MonoBehaviour
{
    public float movementSpeed = 5f; // Pr�dko�� poruszania si� gracza
    public float sprintSpeed = 10f; // Pr�dko�� sprintu
    public float sprintDuration = 3f; // Czas trwania sprintu (w sekundach)
    public float sprintCooldown = 5f; // Czas odnowienia sprintu (w sekundach)
    public float jumpForce = 5f; // Si�a skoku
    public float deceleration = 10f; // Wsp�czynnik zwalniania
    public GameObject platforma = null; // obiekt platformy

    private GameObject aktualnaPlatforma = null; // obiekt platformy, aktualnie dobytej
    private bool isJumping = false; // Czy gracz jest w trakcie skoku
    private bool isSprinting = false; // Czy gracz jest w trakcie sprintu
    private bool isDoubleJumpAvailable = false; // Czy mo�liwy jest podw�jny skok
    private float sprintTimer = 0f; // Licznik czasu sprintu
    private float sprintCooldownTimer = 0f; // Licznik czasu odnowienia sprintu
    private float totalJumpDistance = 0f; // Ca�kowita d�ugo�� wszystkich wykonanych skok�w
    private float totalSprintTime = 0f; // Ca�kowity czas przebyty w sprincie
    private float totalDistance = 0f; // Ca�kowicie pokonany dystans
    private float lastScanDistance = -999f;
    private float gamePoints = 0f;
    private Vector3 currentVelocity; // Aktualna pr�dko�� gracza
    private Rigidbody rb;
    private Renderer playerRenderer;
    private Color originalColor; // Oryginalny kolor obiektu gracza

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRenderer = GetComponent<Renderer>();
        originalColor = playerRenderer.material.color;
    }

    void Update()
    {
        // Pobieranie wej�cia od gracza w osiach poziomych i pionowych
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (aktualnaPlatforma) {
            totalDistance = rb.transform.position.z;
            float floatValue = aktualnaPlatforma.gameObject.GetComponent<menager>().maxTuch;

            //generowanie dalszej mapy je�li gracz leci do przodu
            if (totalDistance > (lastScanDistance + 1) && floatValue > 0)
            {
                float liczdaleko = totalDistance / 100;
                if (liczdaleko <= 0) { liczdaleko = 0; }
                //Debug.Log(totalDistance + " test: " + liczdaleko);

                float rng1 = Random.Range(-16, 16) + liczdaleko;
                float rng2 = Random.Range(59, 69);
                float rng3 = Random.Range(-2, +3);

                // aktualizacja pozycji skanu
                lastScanDistance = totalDistance;

                // nowy obiekt robimy se
                GameObject newObject = Instantiate(platforma, transform.position, transform.rotation);
                Vector3 planePos = aktualnaPlatforma.gameObject.transform.position;
                Vector3 panelRoot = aktualnaPlatforma.gameObject.transform.eulerAngles;

                // nadajemy mu nowe wartosci (jesli bedzie mniej jak 90, dodaj +1)
                if (planePos.y <= 90) { planePos.y = planePos.y + 1; }
                newObject.transform.position = new Vector3(planePos.x + rng1, planePos.y + rng3, planePos.z + rng2);

                if (panelRoot.x > 16)
                {
                    panelRoot.x = panelRoot.x - 1;
                }
                else if(panelRoot.x < -16) 
                {
                    panelRoot.x = panelRoot.x + 1;
                }
                else
                {
                    panelRoot.x = panelRoot.x + Random.Range(-1, 1);
                }

                if (panelRoot.y > 16)
                {
                    panelRoot.y = panelRoot.y - 1;
                }
                else if (panelRoot.y < -16)
                {
                    panelRoot.y = panelRoot.y + 1;
                }
                else
                {
                    panelRoot.y = panelRoot.y + Random.Range(-1, 1);
                }

                if (panelRoot.z > 16)
                {
                    panelRoot.z = panelRoot.z - 1;
                }
                else if (panelRoot.z < -10)
                {
                    panelRoot.z = panelRoot.z + 1;
                }
                else
                {
                    panelRoot.z = panelRoot.z + Random.Range(-1, 1);
                }

                newObject.transform.eulerAngles = new Vector3(panelRoot.x, panelRoot.y, panelRoot.z);

                menager script = aktualnaPlatforma.gameObject.GetComponent<menager>();
                script.maxTuch = 0;
            }
        }

        // Obliczanie wektora ruchu na podstawie wej�cia gracza
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        // Normalizowanie wektora ruchu, aby zachowa� sta�� pr�dko��, niezale�nie od kierunku
        movement.Normalize();

        // Obliczanie wektora pr�dko�ci na podstawie wektora ruchu i pr�dko�ci poruszania si�
        float currentSpeed = isSprinting ? sprintSpeed : movementSpeed;
        Vector3 targetVelocity = movement * currentSpeed;

        // �agodne zwalnianie pr�dko�ci, gdy nie naciskamy przycisk�w poruszania si�
        if (movement == Vector3.zero)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * deceleration);
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * deceleration);
        }

        // Przypisywanie pr�dko�ci obiektowi
        rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);


        // Je�li spanie poni�ej warto�ci
        // Zresetuj byt do punktu wyjscia
        float height = rb.transform.localPosition.y;
        if (height <= 89 && aktualnaPlatforma != null) {
            // RESET do punktu wyjscia gracza
            Vector3 platPos = aktualnaPlatforma.transform.position;

            Vector3 newPosition = new Vector3(platPos.x, platPos.y+2f, platPos.z); 
            rb.transform.position = newPosition;
        }

        // Skakanie
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJumping)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isJumping = true;
                isDoubleJumpAvailable = true;
            }
            else if (isDoubleJumpAvailable)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Zatrzymaj poprzedni� pr�dko�� wertykaln�
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isDoubleJumpAvailable = false;
            }
        }

        // Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting && sprintCooldownTimer <= 0f)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
            playerRenderer.material.color = Color.red; // Zmiana koloru na czerwony
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            totalSprintTime += Time.deltaTime; // Dodaj czas sprintu do ca�kowitego czasu sprintu
            if (sprintTimer <= 0f)
            {
                isSprinting = false;
                sprintCooldownTimer = sprintCooldown;
                playerRenderer.material.color = originalColor; // Przywr�cenie oryginalnego koloru
            }
        }

        if (sprintCooldownTimer > 0f)
        {
            sprintCooldownTimer -= Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Sprawdzanie, czy gracz dotyka pod�o�a
        if (collision.gameObject.CompareTag("Ground"))
        {
            float point = collision.gameObject.GetComponent<menager>().pointSfrom;
            menager script = collision.gameObject.GetComponent<menager>();
            GameObject platfromnowe = collision.gameObject;
            aktualnaPlatforma = platfromnowe;

            if (point > 0) {
                gamePoints += 1;
                script.pointSfrom = 0;
            }

            // pozosta�e mechaniki
            isJumping = false;
            isDoubleJumpAvailable = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Zliczanie d�ugo�ci wszystkich wykonanych skok�w
        if (isJumping && collision.gameObject.CompareTag("Ground"))
        {
            totalJumpDistance += rb.velocity.magnitude * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Zwi�kszanie punktacji za pokonany dystans
        if (other.CompareTag("Pickup"))
        {
            totalDistance += Vector3.Distance(transform.position, other.transform.position);
            Destroy(other.gameObject);
        }
    }

    void OnGUI()
    {
        // Wy�wietlanie danych w konsoli
        GUILayout.Label("Total Distance: " + totalDistance.ToString("F2"));
        GUILayout.Label("Punkty gry: " + gamePoints.ToString("F2"));
    }
}