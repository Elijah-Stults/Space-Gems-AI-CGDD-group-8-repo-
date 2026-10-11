using System.Collections;
using System.IO;
using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{
    public float rotationSpeed;  //speed for patrol rotation
    public float rotationAmount; //angle amount for patrol rotation

    private float initialRotation;

    public Light detectorLight;
    
    public float scanTimer = 0.5f; //timer for yellow scan for player
    public float alertTimer = 5f;  //timer for amount of time that an active alert will exist
    public bool playerDetected = false;
    public bool redLightActive = false;
    private Coroutine scanCoroutine;

    public Transform player;

    public SecurityGuardController securityGuard;

    Vector3 playerPos;

    public bool showAlert = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialRotation = transform.eulerAngles.y; //Gets the starting rotation of the camera
        detectorLight.color = Color.green;
        showAlert = false;
    }

    // Update is called once per frame
    void Update()
    {
        patrol();
        Vector3 dirToPlayer = player.position - detectorLight.transform.position; 

        float dist = dirToPlayer.magnitude; //distance from the spotlight to the player

        float angle = Vector3.Angle(detectorLight.transform.forward, dirToPlayer); //gets the angle

        bool detected = false;

        if(dist <= detectorLight.range && angle <= detectorLight.spotAngle / 2f)
        {
            RaycastHit hitData;

            //casts a ray from the camera to the player once the player enters the spotlight
            if (Physics.Raycast(
                detectorLight.transform.position, 
                dirToPlayer.normalized,
                out hitData, 
                dist))
            {
                if (hitData.collider.CompareTag("Player"))
                {
                    detected = true;
                    playerPos = hitData.transform.position; //players position at time of detection
                } 
            }
        }

        showAlert = redLightActive;
        checkPlayerDetected(detected);
    }

    IEnumerator scanningTimer()
    {
        detectorLight.color = Color.yellow;

        yield return new WaitForSeconds(scanTimer); //scan time

        if(playerDetected)
        {
            redLightActive = true;
            detectorLight.color = Color.red;

            alertGuard(playerPos); //alerts the security guard with the players Vector3 position
            showAlert = true;

            yield return new WaitForSeconds(alertTimer); //how long the alert will last

            redLightActive = false;
            showAlert = false;
        }

        scanCoroutine = null;

        if (playerDetected)
        {
            scanCoroutine = StartCoroutine(scanningTimer());
        }
        else
        {
            detectorLight.color = Color.green;
        }
    }

    //makes sure that the player is being detected and only alerts when player has been in the light for too long
    void checkPlayerDetected(bool detected)
    {
        playerDetected = detected;
        if(redLightActive)
        {
            return;
        }

        if(detected)
        {
            if(scanCoroutine == null)
            {
                scanCoroutine = StartCoroutine(scanningTimer());
            }
        } else
        {
            if(scanCoroutine != null)
            {
                StopCoroutine(scanCoroutine);
                scanCoroutine = null;
            }

            detectorLight.color = Color.green;
        }
    }

    //Has the camera/spotlight rotate based on the parameters
    void patrol()
    {
        float rotating = Mathf.Sin(Time.time * rotationSpeed * Mathf.Deg2Rad) * rotationAmount;

        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x, 
            initialRotation + rotating, 
            transform.eulerAngles.z);
    }

    //Alerts guard of player detection
    void alertGuard(Vector3 playerAPos)
    {
        securityGuard.playerAlertPos = playerAPos;
        securityGuard.isAlerted = true;
    }

    private void OnGUI()
    {
        /*
         * Option 1 for alert message: top-middle of the screen
         * 
         * GUIStyle style = new GUIStyle();
         * style.fontSize = 24;
         * style.normal.textColor = Color.red;
         * style.fontStyle = FontStyle.Bold;
         * 
         * GUI.Label(
         *     new Rect(580, 50, 500, 40),
         *     "Alert, You Have Been Spotted!",
         *     style
         * );
        */
        if (showAlert)
        {
            //Option 2 for alert message: bottom-left of the screen
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.red;
            style.fontStyle = FontStyle.Bold;

            GUI.Label(
                new Rect(100, 450, 500, 40),
                "Alert, You Have Been Spotted!",
                style
            );
        }
    }
}
