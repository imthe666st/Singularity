﻿using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Singularity.Scripting
{
	/// <summary>
	///     Template for scripting
	/// </summary>
	public abstract class ScriptingTemplate
	{
		public SingularityGame Game { get; private set; }
		public GameScene Scene { get; private set; }

		/// <summary>
		///     Initialize - gets called after construction
		/// </summary>
		/// <param name="game">SingularityGame</param>
		public virtual void Init(SingularityGame game, GameScene scene)
		{
			Game = game;
			Scene = scene;
		}

		/// <summary>
		///     Return Settings for setting up Scene - if a value is left null it will use default
		/// </summary>
		/// <returns>Settings of Scene</returns>
		public abstract SceneSettings GetSettings();

		/// <summary>
		///     Add all GameObjects for this scene to objectList
		/// </summary>
		/// <param name="objectList">a complete instanciated objectList. don't delete. just add</param>
		/// <param name="entranceId"></param>
		public abstract List<GameObject> AddGameObjects(int entranceId);

		/// <summary>
		///     Define lighting
		/// </summary>
		/// <param name="effect">BasicEffect</param>
		public abstract void AddLightningToEffect(Effect effect);

		/// <summary>
		///		Define own draw-Method
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="finalTarget"></param>
		public virtual void Draw(SpriteBatch spriteBatch, RenderTarget2D finalTarget) { }
	}
}