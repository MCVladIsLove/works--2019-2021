using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game1
{
    class RelationsManager
    {
        Vector2 _pos;
        float _distanceBetweenObj;
        Priority _priority;
        MapElements _sourceOfSound;

        public MapElements SourceOfSound { get { return _sourceOfSound; } }
        public Vector2 Position { get { return _pos; } }
        public Priority Priority {get { return _priority; } set { _priority = value; } }
        public float Distance {get { return _distanceBetweenObj; } }
        public RelationsManager(MapElements sourceOfSound)
        {
            _pos = sourceOfSound.Position;
            _sourceOfSound = sourceOfSound;
            _priority = Priority.Track;
        }
        public void CalculateDistance(Vector2 listenerPos)
        {
            _distanceBetweenObj = MathF.Sqrt(MathF.Pow(_sourceOfSound.Position.X - listenerPos.X, 2) + MathF.Pow(_sourceOfSound.Position.Y - listenerPos.Y, 2));
        }
    }
}
