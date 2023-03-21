using UnityEngine;


namespace VectorVanguard
{

    /// <summary>
    /// A class to represent a 2D polar coordinate system.
    /// </summary>
    [SerializeField]
    public class PolarCoordinates2D
    {
        [SerializeField] private Vector2 _origin = Vector2.zero;
        [SerializeField] private float _radius;
        [SerializeField][Range(0,360)] private float _angle;
        
        public Vector2 Origin
        {
            get => _origin;
            set => _origin = value;
        }
        
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        
        public float Angle
        {
            get => _angle;
            set => _angle = value;
        }
        
        
        public PolarCoordinates2D()
        {
            _origin = Vector2.zero;
            _radius = 0;
            _angle = 0;
        }
        public PolarCoordinates2D(Vector2 origin, float radius, float angle)
        {
            _origin = origin;
            _radius = radius;
            _angle = angle;
        }
        
        public Vector2 ToCartesian()
        {
            return new Vector2(
                _origin.x + _radius * Mathf.Cos(_angle * Mathf.Deg2Rad),
                _origin.y + _radius * Mathf.Sin(_angle * Mathf.Deg2Rad)
            );
        }
        
        
    }

}