﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Code.Collisions;
using Singularity.Code.GameObjects;

namespace Singularity.Code
{
	/// <summary>
	/// A GameObject can be any object in a GameScene.
	/// </summary>
	public abstract class GameObject
	{
		
		public Model Model { get; private set; } // Model of the entity. Is Null if the object shall not be rendered.
		public Vector3 Position { get; private set; } // Current position of the model
		public Vector3 Rotation { get; private set; } // Current rotation of the model
		public Vector3 Scale { get; private set; } // Scale of the model
		public Collision Collision { get; private set; }

		public GameObject ParentObject { get; private set; } // Parent Object. This object will be in the ChildObjects of the Parent.
		public List<GameObject> ChildObjects { get; private set; } // Child Objects

		public String DebugName { get; private set; } // Used for debugging.

		public float ModelRadius { get; private set; }

		private readonly List<Action<GameScene, GameObject, GameTime>> ObjectScripts; // Basic Actionscripts

		/// <summary>
		/// Initializing Constructor
		/// Sets default values for all properties
		/// </summary>
		protected GameObject()
		{
			// Setting default values for all members
			this.Position = new Vector3();
			this.Rotation = new Vector3();
			this.Scale = Vector3.One;

			this.ParentObject = null;
			this.ChildObjects = new List<GameObject>();

			this.ObjectScripts = new List<Action<GameScene, GameObject, GameTime>>();

		}

		#region Builder Pattern

		#region SetModel

		/// <summary>
		/// Sets the <see cref="Model"/> for the <see cref="GameObject"/>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public GameObject SetModel(Model model)
		{
			this.Model = model;

			var center = this.GetHierarchyPosition();

			// now get max(r + r(v))
			float rm = 0.0f;
			foreach (var mesh in this.Model.Meshes)
			{
				// get distance
				var bs = mesh.BoundingSphere;
				var dist = (bs.Center - center).Length() + bs.Radius;

				if (dist > rm) rm = dist;
			}

			this.ModelRadius = rm;

			if (this is ICollidable || this is ICollider) // everything that has something to do with collisions gets a sphere at the beginning
				this.Collision = new SphereCollision(this);
			return this;
		}

		/// <summary>
		/// Sets the <see cref="Model"/> for the <see cref="GameObject"/>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public GameObject SetModel(String model)
		{
			return this.SetModel(ModelManager.GetModel(model));
		}

		#endregion

		#region SetPosition

		/// <summary>
		/// Sets the <see cref="Position"/> by calling <seealso cref="SetPosition(Vector3)"/> with the specified values.
		/// The Z part of the <see cref="Vector3"/> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject SetPosition(float x, float y) => SetPosition(new Vector3(x, y, 0));
		
		/// <summary>
		/// Sets the <see cref="Position"/> by calling <seealso cref="SetPosition(Vector3)"/> with the specified values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetPosition(float x, float y, float z) => SetPosition(new Vector3(x, y, z));

		/// <summary>
		/// Sets the <see cref="Position"/>
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public GameObject SetPosition(Vector3 position)
		{
			this.Position = position;
			
			return this;
		}

		#endregion

		#region AddPosition

		/// <summary>
		/// Modifies the <see cref="Position"/> by calling <seealso cref="AddPosition(Vector3)"/> with the specified values.
		/// The Z part of the <see cref="Vector3"/> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject AddPosition(float x, float y) => AddPosition(new Vector3(x, y, 0));

		/// <summary>
		/// Modifies the <see cref="Position"/> by calling <seealso cref="AddPosition(Vector3)"/> with the specified values
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddPosition(float x, float y, float z) => AddPosition(new Vector3(x, y, z));

		/// <summary>
		/// Modifies the <see cref="Position"/> by adding the Vectors
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public GameObject AddPosition(Vector3 position)
		{
			this.Position += position;
			return this;
		}

		#endregion

		#region SetRotation

		/// <summary>
		/// Sets the <see cref="Rotation"/> by calling <seealso cref="SetPosition(Vector3)"/>
		/// The Z value of the <see cref="Vector3"/> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject SetRotation(float x, float y) => SetRotation(new Vector3(x, y, 0));

		/// <summary>
		/// Sets the <see cref="Rotation"/> by calling <seealso cref="SetPosition(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetRotation(float x, float y, float z) => SetRotation(new Vector3(x, y, z));
		/// <summary>
		/// Sets the <see cref="Rotation"/>
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject SetRotation(Vector3 rotation)
		{
			this.Rotation = rotation;
			return this;
		}

		#endregion

		#region AddRotation


		/// <summary>
		/// Modifies the <see cref="Rotation"/> by calling <seealso cref="AddRotation(Vector3)"/>
		/// The Z value of the <see cref="Vector3"/> will be 0.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public GameObject AddRotation(float x, float y) => AddRotation(new Vector3(x, y, 0));
		/// <summary>
		/// Modifies the <see cref="Rotation"/> by calling <seealso cref="AddRotation(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddRotation(float x, float y, float z) => AddRotation(new Vector3(x, y, z));
		/// <summary>
		/// Modifies the <see cref="Rotation"/> by adding both <see cref="Vector3"/>
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public GameObject AddRotation(Vector3 rotation)
		{
			this.Rotation += rotation;
			return this;
		}

		#endregion

		#region SetScale

		/// <summary>
		/// Sets the <see cref="Scale"/> by calling <seealso cref="SetScale(Vector3)"/>
		/// All dimensions are set to the parameter.
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject SetScale(float scale) => SetScale(scale, scale, scale);

		/// <summary>
		/// Sets the <see cref="Scale"/> by calling <seealso cref="SetScale(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject SetScale(float x, float y, float z) => SetScale(new Vector3(x, y, z));
		/// <summary>
		/// Sets the <see cref="Scale"/>
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject SetScale(Vector3 scale)
		{
			this.Scale = scale;
			return this;
		}

		#endregion

		#region MultiplyScale
		/// <summary>
		/// Multiplies <see cref="Scale"/> by calling <seealso cref="MultiplyScale(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(float x, float y, float z) => MultiplyScale(new Vector3(x, y, z));
		/// <summary>
		/// Multiplies <see cref="Scale"/> with <paramref name="scale"/>
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject MultiplyScale(Vector3 scale)
		{
			this.Scale *= scale;
			return this;
		}
		#endregion

		#region AddScale

		/// <summary>
		/// Modifies <see cref="Scale"/> by calling <seealso cref="AddScale(Vector3)"/>
		/// All dimension of the <see cref="Vector3"/> will be set to <paramref name="scale"/>
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject AddScale(float scale) => AddScale(new Vector3(scale));
		/// <summary>
		/// Modifies <see cref="Scale"/> by calling <seealso cref="AddScale(Vector3)"/>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public GameObject AddScale(float x, float y, float z) => AddScale(new Vector3(x, y, z));
		/// <summary>
		/// Adds <paramref name="scale"/> to the <see cref="Scale"/>
		/// </summary>
		/// <param name="scale"></param>
		/// <returns></returns>
		public GameObject AddScale(Vector3 scale)
		{
			this.Scale += scale;
			return this;
		}

		#endregion

		#region SetParent
		/// <summary>
		/// Sets the <see cref="ParentObject"/>
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		public GameObject SetParent(GameObject parent)
		{
			this.ParentObject = parent;
			parent.ChildObjects.Add(this);

			return this;
		}
		#endregion

		#region AddScript

		/// <summary>
		/// Adds a <see cref="Action"/> to the Scripts, which will be executes after <seealso cref="Update"/> is called.
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		public GameObject AddScript(Action<GameScene, GameObject, GameTime> script)
		{
			this.ObjectScripts.Add(script);

			return this;
		}

		#endregion

		#region AddChild

		/// <summary>
		/// Adds a Childobject which will move relative to this <see cref="GameObject"/>
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public GameObject AddChild(GameObject child)
		{
			this.ChildObjects.Add(child);
			child.ParentObject = this;
			return this;
		}
		#endregion

		#region SetDebugName
		/// <summary>
		/// Sets the <see cref="DebugName"/> for testing.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public GameObject SetDebugName(String name)
		{
			this.DebugName = name;
			return this;
		}
		#endregion

		#region SetCollision

		public GameObject SetCollision(Collision collision)
		{
			this.Collision = collision;
			return this;
		}

		#endregion

		#endregion

		/// <summary>
		/// Return the multiplies <see cref="Scale"/> from this <see cref="GameObject"/> and the <see cref="ParentObject"/> <seealso cref="GetHierarchyScale()"/>
		/// </summary>
		/// <returns></returns>
		public Vector3 GetHierarchyScale()
		{
			if (this.ParentObject == null) return this.Scale;
			return this.Scale * this.ParentObject.GetHierarchyScale();
		}
		
		/// <summary>
		/// Return the added <see cref="Position"/> from this <see cref="GameObject"/> and the <see cref="ParentObject"/> <seealso cref="GetHierarchyPosition()"/>
		/// </summary>
		/// <returns></returns>
		public Vector3 GetHierarchyPosition()
		{
			if (this.ParentObject == null) return this.Position;
			return this.Position + this.ParentObject.GetHierarchyPosition();
		}

		
		/// <summary>
		/// Gets all <see cref="Action"/>scripts set to this <see cref="GameObject"/>
		/// </summary>
		/// <returns></returns>
		public List<Action<GameScene, GameObject, GameTime>> GetScripts()
		{
			return this.ObjectScripts;
		}

		// By https://pastebin.com/47vwJWSc

		/// <summary>
		/// Calculate <seealso cref="BoundingBox"/> for this 
		/// </summary>
		/// <returns></returns>
		public BoundingBox GetBoundingBox()
		{
			return GetBoundingBox(
				this.Model,
				Matrix.CreateRotationX(this.Rotation.X) * Matrix.CreateRotationY(this.Rotation.Y) * Matrix.CreateRotationZ(this.Rotation.Z)
				* Matrix.CreateScale(this.GetHierarchyScale())
			);
		}

		/// <summary>
		/// Calculate <seealso cref="BoundingBox"/> for <seealso cref="Microsoft.Xna.Framework.Graphics.Model"/>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="worldTransformation"></param>
		/// <returns></returns>
		public static BoundingBox GetBoundingBox(Model model, Matrix worldTransformation)
		{

			if (model == null) return new BoundingBox();

			// Initialize minimum and maximum corners of the bounding box to max and min values
			Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			// For each mesh of the model
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					// Vertex buffer parameters
					int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
					int vertexBufferSize = meshPart.NumVertices * vertexStride;

					// Get vertex data as float
					float[] vertexData = new float[vertexBufferSize / sizeof(float)];
					meshPart.VertexBuffer.GetData<float>(vertexData);

					// Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
					for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
					{
						Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransformation);

						min = Vector3.Min(min, transformedPosition);
						max = Vector3.Max(max, transformedPosition);
					}
				}
			}

			// Create and return bounding box
			return new BoundingBox(min, max);
		}

		#region Abstract Methods

		/// <summary>
		/// Calls <seealso cref="Update"/>, and calls back to the scene. 
		/// After that all <see cref="ChildObjects"/> will be updated.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="gameTime"></param>
		public void UpdateLogic(GameScene scene, GameTime gameTime)
		{
			// get a copy of the position
			var position = this.GetHierarchyPosition();

			Update(scene, gameTime);
			
			// check if we are even able to stay here.
			scene.HandleCollision(this, position);

			// execute scripts
			foreach (var actionScript in this.ObjectScripts) actionScript(scene, this, gameTime);


			// did we move?
			if (this.GetHierarchyPosition() != position)
			{
				// we have to talk to the scene about the movement. 
				scene.MoveOctree(this, position);
			} 


			foreach (GameObject obj in this.ChildObjects) obj.UpdateLogic(scene, gameTime);
		}

		/// <summary>
		/// Updates the <see cref="GameObject"/>
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="gameTime"></param>
		public abstract void Update(GameScene scene, GameTime gameTime);

		/// <summary>
		/// Calls <seealso cref="Draw"/>
		/// After that draws all <see cref="ChildObjects"/>
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="spriteBatch"></param>
		public void DrawLogic(GameScene scene, SpriteBatch spriteBatch)
		{
			//Console.WriteLine($"Drawing, Position: {this.Position}");
			Draw(scene);
			Draw2D(spriteBatch);
			foreach (GameObject obj in this.ChildObjects) obj.DrawLogic(scene, spriteBatch);
		}

		protected virtual void Draw2D(SpriteBatch spriteBatch) { }

		/// <summary>
		/// Checks if there is a <see cref="Model"/> to draw and draws it.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="spriteBatch"></param>
		private void Draw(GameScene scene)
		{
			if (this.Model == null) return; // No model means it can't be rendered.

			// copy the scale of bones from the model to apply it later.
			var transformMatrices = new Matrix[this.Model.Bones.Count];
			this.Model.CopyAbsoluteBoneTransformsTo(transformMatrices);

			foreach (ModelMesh mesh in this.Model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					// calculating the full rotation of our object.
					//Console.WriteLine($"POS: {this.GetHierarchyPosition().X} {this.GetHierarchyPosition().Y} {this.GetHierarchyPosition().Z}");

					var position = this.GetHierarchyPosition();
					position.Y = -position.Y;

					Matrix totalRotation = Matrix.CreateRotationX(this.Rotation.X) * Matrix.CreateRotationY(this.Rotation.Y) * Matrix.CreateRotationZ(this.Rotation.Z);

					effect.World = transformMatrices[mesh.ParentBone.Index]
					               * Matrix.CreateScale(this.GetHierarchyScale())
					               * Matrix.CreateTranslation(position)
					               * totalRotation;

					effect.View = scene.GetViewMatrix();
					effect.Projection = scene.GetProjectionMatrix();

					effect.EnableDefaultLighting();
					effect.LightingEnabled = true; // Turn on the lighting subsystem.

					//effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.2f, 0.2f); // some diffuse light
					//effect.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // coming along the x-axis
					//effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f); // a tad of specularity]
					scene.AddLightningToEffect(effect);

					//effect.EmissiveColor = new Vector3(1, 0, 0); // Sets some strange emmissive lighting.  This just looks weird.

				}

				mesh.Draw();
			}

		}

		#endregion

		#region Events

		// currently unused.
		//protected event EventHandler<GameObjectCollisionEvent> OnCollision;

		//public virtual void OnGameObjectCollision(GameObject gameObject, GameScene scene, Vector3 movement) =>
		//	OnGameObjectCollision(new GameObjectCollisionEvent(gameObject, scene, movement));

		//public virtual void OnGameObjectCollision(GameObjectCollisionEvent e) => 
		//	OnCollision?.Invoke(this, e);

		#endregion

	}
}
