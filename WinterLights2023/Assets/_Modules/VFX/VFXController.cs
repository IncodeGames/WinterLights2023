using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [SerializeField] private GameObject renderCamera;
    [SerializeField] private VisualEffect currentEffect;
    [SerializeField] private Transform turbTransform;

    [Header("Tableau One")]
    [SerializeField] private GameObject tableauOne;
    [SerializeField] private Transform rocketOffset;
    [SerializeField] private BezierSolution.BezierWalkerWithSpeed bezierWalker;
    [SerializeField] private GameObject rocketModelRoot;
    private MeshRenderer[] rocketRenderers;

    [Header("Tableau Two")]
    [SerializeField] private GameObject tableauTwo;
    [SerializeField] private Transform lightPositionTwo;
    [SerializeField] private Transform renderCamPositionTwo;

    [Header("Tableau Three")]
    [SerializeField] private GameObject tableauThree;
    [SerializeField] private Transform lightPositionThree;

    [Header("General")]
    [SerializeField] private float tableauSwapTime = 30.0f;
    [SerializeField] private float dragStrength;

    [SerializeField] private Transform lightTransform;


    private Vector3 turbStartPosition;
    
    private int lastTableauIndex = -1;
    private int currentTableauIndex = 0;
    private float lastTableauSwapTime = 0.0f;

    private void SetTurbulenceDrag(float drag)
    {
        currentEffect.SetFloat("Drag", drag);
    }

    private void SetLifetimeRange(float min, float max)
    {
        Vector2 minMax = Vector2.zero;
        minMax.x = min;
        minMax.y = max;
        currentEffect.SetVector2("LifetimeRange", minMax);
    }

    private void SetGravityStrength(float gravity)
    {
        currentEffect.SetFloat("GravityStrength", gravity);
    }

    private void UpdateRocketColor(float tValue)
    {
        float lerpValue = Mathf.Sin(tValue * Mathf.PI);
        rocketRenderers[0].sharedMaterial.color = Color.Lerp(Color.white, Color.black * 0.25f, lerpValue);
    }
    
    void Start()
    {
        rocketRenderers = rocketModelRoot.GetComponentsInChildren<MeshRenderer>();
        turbStartPosition = turbTransform.position;
    }

    private void UpdateTableaus(int currentIndex)
    {
        if (currentIndex == 0)
        {
            SetTurbulenceDrag(0);
            SetGravityStrength(0);

            tableauOne.SetActive(true);
            tableauThree.SetActive(false);

            lightTransform.parent = rocketOffset;
            lightTransform.localPosition = Vector3.zero;

            SetLifetimeRange(4f, 7f);
        }
        else if (currentIndex == 1)
        {
            SetTurbulenceDrag(0);

            tableauOne.SetActive(false);
            tableauTwo.SetActive(true);

            lightTransform.parent = null;
            lightTransform.position = lightPositionTwo.position;

            renderCamera.transform.parent = null;
            renderCamera.transform.position = renderCamPositionTwo.position;

        }
        else if (currentIndex == 2)
        {
            SetTurbulenceDrag(0);

            tableauTwo.SetActive(false);
            tableauThree.SetActive(true);
            
            lightTransform.parent = null;
            lightTransform.position = lightPositionThree.position;
        }
    }

    void Update()
    {
        float lerpValue = ((Time.time - lastTableauSwapTime) / tableauSwapTime);

        if (currentTableauIndex == 0)
        {
            UpdateRocketColor(bezierWalker.NormalizedT);
            if (lerpValue > 0.5f)
            {
                SetTurbulenceDrag((lerpValue - 0.5f) * 2f);
            }

            if (lerpValue > 0.75f)
            {
                SetLifetimeRange(0.5f, 1f);
            }
        }
        else if (currentTableauIndex == 1)
        {
            // if (lerpValue > 0.4f)
            // {
            //     // SetTurbulenceDrag(2.0f);
            //     SetGravityStrength(1.0f);
            // }
            if (lerpValue > 0.75f)
            {
                // SetGravityStrength(0.0f);
                SetTurbulenceDrag(3.0f);
            }
        }
        else if (currentTableauIndex == 2)
        {
            if (lerpValue > 0.5f)
            {
                SetTurbulenceDrag((lerpValue - 0.5f) * 2f);
            }

            if (lerpValue > 0.8f)
            {
                SetLifetimeRange(1f, 3f);
                SetGravityStrength(0.0025f);
            }
        }

        if (Time.time - lastTableauSwapTime > tableauSwapTime)
        {

            currentTableauIndex = (currentTableauIndex + 1) % 3;
            lastTableauSwapTime = Time.time;
        }

        if (lastTableauIndex != currentTableauIndex)
        {
            UpdateTableaus(currentTableauIndex);
            lastTableauIndex = currentTableauIndex;
        }
        // SetTurbulenceDrag(Mathf.Sin(Time.time) * dragStrength);
    }
}
