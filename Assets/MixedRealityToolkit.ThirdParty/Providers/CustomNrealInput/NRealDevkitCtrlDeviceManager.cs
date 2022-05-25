using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using NRKernal;

namespace SSI.MixedReality.Toolkit.Nreal.Input
{
    [MixedRealityDataProvider(
    typeof(IMixedRealityInputSystem),
    SupportedPlatforms.Android | 
    SupportedPlatforms.WindowsEditor | 
    SupportedPlatforms.MacEditor | 
    SupportedPlatforms.LinuxEditor,
    "Nreal Devkit Ctrl Device Manager")]
    public class NrealDevkitCtrlDeviceManager : BaseInputDeviceManager,
        IMixedRealityCapabilityCheck
    {

        private NRealDevkitController controller = null;

        public NrealDevkitCtrlDeviceManager(
            IMixedRealityInputSystem inputSystem, 
            string name, 
            uint priority, 
            BaseMixedRealityProfile profile): base(inputSystem, name, priority, profile) 
        { 

        }

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            // Puck is a motion controller.
            return (capability == MixedRealityCapability.MotionController);
        }


        #endregion IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();
        }


        /// <inheritdoc />
        public override void Update()
        {
            base.Update();


                if (controller==null)
                {
                    NRInput.LaserVisualActive = false;
                    NRInput.ReticleVisualActive = false;

                    var inputSystem = Service as IMixedRealityInputSystem;
                    var handedness = NRInput.DomainHand == ControllerHandEnum.Left ? Handedness.Left : Handedness.Right;
                    var pointers = RequestPointers(SupportedControllerType.ArticulatedHand, handedness);
                    var inputSource = inputSystem?.RequestNewGenericInputSource($"Nreal Light Controller", pointers, InputSourceType.Hand);
                    controller = new NRealDevkitController(Microsoft.MixedReality.Toolkit.TrackingState.NotTracked, handedness, inputSource);
                    // controller.SetupConfiguration(typeof(NRealDevkitController));
                    for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
                    {
                        controller.InputSource.Pointers[i].Controller = controller;
                    }
                    inputSystem.RaiseSourceDetected(controller.InputSource, controller);
                }
                controller.UpdateState();

                // // Change RaycastMode
                // CK: 22/04/2022 -- Changed this code to AppManager. 
                // if (NRInput.GetButtonUp(ControllerButton.APP))
                // {
                //     var inputSystem = Service as IMixedRealityInputSystem;
                //     inputSystem.RaiseSourceLost(controller.InputSource, controller);
                //     inputSystem.RaiseSourceDetected(controller.InputSource, controller);
                //     NRInput.RaycastMode = NRInput.RaycastMode == RaycastModeEnum.Laser ? RaycastModeEnum.Gaze : RaycastModeEnum.Laser;
                // }



            //using (UpdatePerfMarker.Auto())
            //{
                // if (NRInput.IsInitialized)
                // {

                    // UpdateNrealTrackedHands(NRInput.Hands.GetHandState(HandEnum.LeftHand).isTracked, NRInput.Hands.GetHandState(HandEnum.RightHand).isTracked);
                
                    // // Update the hand/hands that are in trackedhands
                    // foreach (KeyValuePair<Handedness, NrealController> hand in trackedHands)
                    // {           
                    //     hand.Value.UpdateState();
                    // }
                // }
            // }
        }

    }
}