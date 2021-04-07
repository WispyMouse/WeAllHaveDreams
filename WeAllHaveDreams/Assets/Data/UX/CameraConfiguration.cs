using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    [System.Serializable]
    public class CameraConfiguration : ConfigurationData
    {
        /// <summary>
        /// The speed that the camera pans at, at base.
        /// This is measured in tiles per second. "5" means you'd move the screen 5 tiles per second of panning.
        /// </summary>
        public float CameraPanningSpeed;
    }
}
