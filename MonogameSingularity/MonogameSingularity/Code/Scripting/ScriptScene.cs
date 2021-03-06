﻿using Microsoft.Xna.Framework.Input;
using Singularity.GameObjects;

namespace Singularity.Scripting
{
	using Microsoft.Xna.Framework.Graphics;

	public class ScriptScene : GameScene
	{
		private readonly string _scriptKey;
		public ScriptingTemplate Script;
		private bool _owndraw;

		public ScriptScene(SingularityGame game, string path, ScriptingTemplate script)
			: base(game,
				path,
				script.GetSettings().SceneSize == null ? 16 : script.GetSettings().SceneSize.Value,
				script.GetSettings().MinPartition == null ? 2 : script.GetSettings().MinPartition.Value,
				script.GetSettings().Precision == null ? 0f : script.GetSettings().Precision.Value)
		{
			_owndraw = script.GetSettings().OwnDraw ?? false;
			_scriptKey = path;
			Script = script;
			Script.Init(game, this);
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			var objs = Script.AddGameObjects(entranceId);
			foreach (var gameObject in objs) AddObject(gameObject);

#if DEBUG
			AddObject(new GameObject().AddScript((scene, o, time) =>
			{
				if (!InputManager.IsKeyPressed(Keys.O)) return;

				ScriptManager.ReloadScript(_scriptKey);
				SceneManager.ChangeScene(_scriptKey, entranceId);
			}));
#endif
		}

		public override void Draw(SpriteBatch spriteBatch, RenderTarget2D finalTarget)
		{
			if(_owndraw)
				Script.Draw(spriteBatch, finalTarget);
			else
				base.Draw(spriteBatch, finalTarget);
		}
	}
}