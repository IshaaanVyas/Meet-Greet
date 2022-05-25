using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

using NRKernal.Record;
using NRKernal;

public class PhotoCapture : MonoBehaviour
{
    /// <summary>
    /// Network manager instance. 
    /// </summary>
    public NetworkManager m_NetworkManager;

    /// <summary> The photo capture object. </summary>
    private NRPhotoCapture m_PhotoCaptureObject;

    /// <summary> The camera resolution. </summary>
    private Resolution m_CameraResolution;

    private bool isOnPhotoProcess = false;

    private bool isEnableTimedCapture = false;

    /// <summary>
    /// TImed loop to keep grabbing frame image. 
    /// </summary>
    private IEnumerator frameGrabberCoroutine;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        //if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        //{
        //    TakeAPhoto();
        //}
    }

    /// <summary> Use this for initialization. </summary>
    void Create(Action<NRPhotoCapture> onCreated)
    {
        if (m_PhotoCaptureObject != null)
        {
            NRDebugger.Info("The NRPhotoCapture has already been created.");
            return;
        }

        // Create a PhotoCapture object
        NRPhotoCapture.CreateAsync(false, delegate (NRPhotoCapture captureObject)
        {
            m_CameraResolution = NRPhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

            if (captureObject == null)
            {
                NRDebugger.Error("Can not get a captureObject.");
                return;
            }

            m_PhotoCaptureObject = captureObject;

            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.cameraResolutionWidth = m_CameraResolution.width;
            cameraParameters.cameraResolutionHeight = m_CameraResolution.height;
            //cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
            cameraParameters.pixelFormat = CapturePixelFormat.JPEG;
            cameraParameters.frameRate = NativeConstants.RECORD_FPS_DEFAULT;
            cameraParameters.blendMode = BlendMode.Blend;

            // Activate the camera
            m_PhotoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (NRPhotoCapture.PhotoCaptureResult result)
            {
                NRDebugger.Info("Start PhotoMode Async");
                if (result.success)
                {
                    onCreated?.Invoke(m_PhotoCaptureObject);
                }
                else
                {
                    isOnPhotoProcess = false;
                    this.Close();
                    NRDebugger.Error("Start PhotoMode faild." + result.resultType);
                }
            }, true);
        });
    }

    /// <summary> Take a photo. </summary>
    void TakeAPhoto()
    {
        if (isOnPhotoProcess)
        {
            NRDebugger.Warning("Currently in the process of taking pictures, Can not take photo .");
            return;
        }

        isOnPhotoProcess = true;
        if (m_PhotoCaptureObject == null)
        {
            this.Create((capture) =>
            {
                capture.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        }
        else
        {
            m_PhotoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
    }

    /// <summary> Executes the 'captured photo memory' action. </summary>
    /// <param name="result">            The result.</param>
    /// <param name="photoCaptureFrame"> The photo capture frame.</param>
    void OnCapturedPhotoToMemory(NRPhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        //var targetTexture = new Texture2D(m_CameraResolution.width, m_CameraResolution.height);
        // Copy the raw image data into our target texture
        //photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        // --->> Uncomment this block to enabel display on screen. 
        //// Create a gameobject that we can apply our texture to
        //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        //quadRenderer.material = new Material(Resources.Load<Shader>("Record/Shaders/CaptureScreen"));

        //var headTran = NRSessionManager.Instance.NRHMDPoseTracker.centerAnchor;
        //quad.name = "picture";
        //quad.transform.localPosition = headTran.position + headTran.forward * 3f;
        //quad.transform.forward = headTran.forward;
        //quad.transform.localScale = new Vector3(1.6f, 0.9f, 0);
        //quadRenderer.material.SetTexture("_MainTex", targetTexture);

        // For sending to server. 

        //// Destroy Texture as we are not using it. 
        //Destroy(targetTexture);

        StartCoroutine(m_NetworkManager.UploadFile(photoCaptureFrame.GetRawData()));
        //StartCoroutine(m_NetworkManager.UploadFile(imgbytes));

        // Release camera resource after capture the photo.
        this.Close();
    }

    private IEnumerator WaitAndGrabFrame(float waitTime)
    {
        while (isEnableTimedCapture)
        {
            // TODO: Capture Frame 
            TakeAPhoto();
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void StartFrameGrabber()
    {
        isEnableTimedCapture = true;
        frameGrabberCoroutine = WaitAndGrabFrame(0.2f);
        StartCoroutine(frameGrabberCoroutine);
    }

    public void StopFrameGrabber()
    {
        // Stop the next coroutine thread if not started. 
        if (frameGrabberCoroutine != null)
        {
            StopCoroutine(frameGrabberCoroutine);
        }
        isEnableTimedCapture = false;
    }

    /// <summary> Closes this object. </summary>
    void Close()
    {
        if (m_PhotoCaptureObject == null)
        {
            NRDebugger.Error("The NRPhotoCapture has not been created.");
            return;
        }
        //// Deactivate our camera
        //m_PhotoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        isOnPhotoProcess = false;
    }

    /// <summary> Executes the 'stopped photo mode' action. </summary>
    /// <param name="result"> The result.</param>
    void OnStoppedPhotoMode(NRPhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        m_PhotoCaptureObject?.Dispose();
        m_PhotoCaptureObject = null;
        isOnPhotoProcess = false;
    }

    /// <summary> Executes the 'destroy' action. </summary>
    void OnDestroy()
    {
        // Shutdown our photo capture resource
        m_PhotoCaptureObject?.Dispose();
        m_PhotoCaptureObject = null;
    }



}
