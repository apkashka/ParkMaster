using UnityEngine;

namespace CodeBase.Static
{
    public static class CashedLayerMasks
    {
        private static int _carLayer;
        private static int _parking;
        private static int _groundLayer;
        private static int _obstacleLayer;

        public static int CarLayer => _carLayer == 0
            ? _carLayer = LayerMask.NameToLayer("Car")
            : _carLayer;

        public static int ParkingLayer => _parking == 0
            ? _parking = LayerMask.NameToLayer("Parking")
            : _parking;

        public static int GroundLayer => _groundLayer == 0
            ? _groundLayer = LayerMask.NameToLayer("Ground")
            : _groundLayer;

        public static int ObstacleLayer => _obstacleLayer == 0
            ? _obstacleLayer = LayerMask.NameToLayer("Obstacle")
            : _obstacleLayer;
    }
}