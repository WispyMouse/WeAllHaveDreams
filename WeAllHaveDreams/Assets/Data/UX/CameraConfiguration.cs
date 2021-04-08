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

        /// <summary>
        /// The orthographic size the camera starts at.
        /// The game is in an "orthographic projection" currently. A value of 5 means ten sprites can fit on screen, vertically.
        /// </summary>
        public float InitialOrthographicSize;

        /// <summary>
        /// The interval that the orthographic size is changed.
        /// This is a pretty soft number, waggle it until it feels right.
        /// </summary>
        public float OrthographicSizeChangePerScrollTick;

        /// <summary>
        /// This is the smallest the game will allow the orthographic size to be.
        /// This can't be 0 or less, mathematically.
        /// </summary>
        public float MinimumOrthographicSize;

        /// <summary>
        /// This is the largest the game will allow the orthographic size to be.
        /// </summary>
        public float MaximumOrthographicSize;
    }
}
