﻿using Microsoft.Xna.Framework;

namespace Singularity.Collisions
{
	public class PlaneCollision : Collision
	{
		public Vector3 Origin { get; private set; }
		public Vector3 SpanVector1 { get; private set; }
		public Vector3 SpanVector2 { get; private set; }
		public Vector3 Normal { get; private set; }

		public PlaneCollision(Vector3 origin, Vector3 spanVector1, Vector3 spanVector2) : base()
		{
			Origin = origin;
			SpanVector1 = spanVector1;
			SpanVector2 = spanVector2;
			Normal = Vector3.Cross(spanVector1, spanVector2);
			
			Normal.Normalize();
		}

		public override object Clone()
		{
			return new PlaneCollision(this.Origin, this.SpanVector1, this.SpanVector2);
		}
	}
}
