﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity.Collisions;
using Singularity.GameObjects.Interfaces;

namespace Singularity.GameObjects
{
	public class BasicCamera : GameObject, ICollider, ICameraController
	{
		private double HorizontalRotation;	// rotation around the z axis
		private double VerticalRotation;	// rotation up and down
		public Boolean Is3DEnabled { get; private set; }

		public BasicCamera() : base()
		{
			this.HorizontalRotation = 0.0d;
			this.VerticalRotation = 0.0d;

			Mouse.SetPosition(200, 200); // capture the mouse

			this.SetCollision(new SphereCollision(0.25f));
		}


		/// <summary>
		/// Enables <see cref="VerticalRotation"/>
		/// </summary>
		/// <param name="enable"></param>
		public void Set3DEnabled(Boolean enable)
		{
			this.Is3DEnabled = enable;
		}

		public override void Update(GameScene scene, GameTime gameTime)
		{
			if (!scene.Game.IsActive) return;


			// Capture Mouse
			MouseState mouseState = Mouse.GetState();
			var dx = mouseState.X - 200;
			var dy = 200 - mouseState.Y;

			this.HorizontalRotation += dx / 100f;

			if (this.Is3DEnabled)
				this.VerticalRotation += dy / 100f;


			Mouse.SetPosition(200, 200);

			// Constraint rotation

			if (HorizontalRotation >= MathHelper.TwoPi) HorizontalRotation -= MathHelper.TwoPi;
			else if (HorizontalRotation < 0f) HorizontalRotation += MathHelper.TwoPi;

			if (VerticalRotation > MathHelper.PiOver2)
				VerticalRotation = MathHelper.PiOver2;
			else if (VerticalRotation < -MathHelper.PiOver2)
				VerticalRotation = -MathHelper.PiOver2;

			// calculate forward vector
			Vector3 target = new Vector3((float)Math.Cos(HorizontalRotation), Is3DEnabled ? (float)Math.Sin(VerticalRotation) : 0f, (float)Math.Sin(HorizontalRotation));

			var movement = new Vector3();
			var ks = Keyboard.GetState();

			target.Normalize();

			// Calculate orthagonal vectors
			Vector3 forward = target;
			forward.Y = 0;
			Vector3 backwards = -forward;

			Vector3 right = new Vector3(forward.Z, 0, -forward.X);
			Vector3 left = -right;

			// normalize vectors

			forward.Normalize();
			backwards.Normalize();

			right.Normalize();
			left.Normalize();

			// Buffer movement
			if (ks.IsKeyDown(Keys.W)) movement += forward;
			if (ks.IsKeyDown(Keys.S)) movement += backwards;

			if (ks.IsKeyDown(Keys.A)) movement += right;
			if (ks.IsKeyDown(Keys.D)) movement += left;

			if (movement.LengthSquared() > 0f) movement.Normalize();

			// test collision
			//if (!scene.DoesCollide(this, movement * (float)gameTime.ElapsedGameTime.TotalSeconds * 5f, 0.125f))
				this.AddPosition(movement * (float)gameTime.ElapsedGameTime.TotalSeconds * 5f);

			// update relative camera
			scene.SetCamera(this.Position, target);

			//Console.WriteLine($"{this.Position.X} - {this.Position.Y} - {this.Position.Z}");
		}

		public void SetCamera(GameScene scene)
		{
			Vector3 target = new Vector3((float)Math.Cos(HorizontalRotation), Is3DEnabled ? (float)Math.Sin(VerticalRotation) : 0f, (float)Math.Sin(HorizontalRotation));

			scene.SetCamera(this.Position, target);
		}
	}
}
