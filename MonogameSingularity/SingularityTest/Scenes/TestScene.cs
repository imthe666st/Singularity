﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Singularity;
using Singularity.Collisions;
using Singularity.Collisions.Multi;
using Singularity.GameObjects;
using Singularity.Utilities;
using SingularityTest.GameObjects;
using SingularityTest.GameObjects.ChildTest;
using SingularityTest.ScreenEffect;

namespace SingularityTest.Scenes
{
	using System.IO;
	using System.Linq;

	public class TestScene : LightGameScene
	{
		public TestScene(SingularityGame game) : base(game, "test", 4096)
		{
			//this.SetCamera(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
			SceneResumeEvent += (s, e) => { Mouse.SetPosition(200, 200); };


			SetLightPosition(new Vector3(-50, 50, 0));
			SetLightDirection(new Vector3(1, -1, 0));

			SetProjectionMatrix(Matrix.CreateOrthographic(100, 100, 0.01f, 200f));
		}

		protected override void AddGameObjects(GameScene previousScene, int entranceId)
		{
			// load the savegame
			var sg = Savegame.GetSavegame();

			Mouse.SetPosition(200, 200);

			var camTarget = sg.IsValidSavegame ? sg.CameraTarget : new Vector3(-1, 0, 0);
			var camPosition = new Vector3();
			if (entranceId == 0)
				camPosition = sg.IsValidSavegame ? sg.Position : new Vector3(0, 0, 10);
			else if (entranceId == 1)
				camPosition = new Vector3(10);

			AddObject(new ModelObject("cubes/cube1").SetPosition(-51, 51, 0).SetScale(0.5f, 0.5f, 0.5f));

			int frame = 0;
			AddObject(new GameObject().SetPosition(-5, -5, -5)
				.AddChild(new GameObject().SetPosition(10, 10, 10)
					.AddChild(new BasicCamera()
				.Set3DEnabled(true)
				.SetCameraTarget(camTarget)
				.SetPosition(camPosition)
				.AddCollisionEvent((e) => Console.WriteLine($"Collision on frame {frame}"))
				.AddScript((scene, obj, time) =>
				{
					frame += 1;
					// enable or disable 3d.
					if (InputManager.IsKeyPressed(Keys.F1)) ((BasicCamera) obj).Set3DEnabled(!((BasicCamera) obj).Is3DEnabled);

					// some more movement options
					if (InputManager.IsKeyDown(Keys.Q) || InputManager.IsDownGamePad(Buttons.LeftThumbstickUp)) obj.AddPosition(new Vector3(0, 5, 0), time);
					if (InputManager.IsKeyDown(Keys.E) || InputManager.IsDownGamePad(Buttons.LeftThumbstickDown)) obj.AddPosition(new Vector3(0, -5, 0), time);

					// screen effects

					if (InputManager.IsKeyPressed(Keys.Y))
						Game.ScreenEffectList.Add(ShakeScreenEffect.GetNewShakeScreenEffect(0.5f, 4).GetEffectData);
					if (InputManager.IsKeyPressed(Keys.X))
						Game.ScreenEffectList.Add(ColorScreenEffect.GetNewColorScreenEffect(1, Color.Red).GetEffectData);
					if (InputManager.IsKeyPressed(Keys.C))
						Game.SaveScreenshot(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));


					if (InputManager.IsKeyPressed(Keys.O))
						SceneManager.AddSceneToStack("transparency");

					if (InputManager.IsKeyPressed(Keys.R)) obj.SetEnableCollision(!obj.EnablePushCollision);

					// savegame stuff
					sg.Position = obj.Position;
					sg.CameraTarget = ((BasicCamera) obj).GetCameraTarget();


					if (InputManager.IsKeyDown(Keys.M) || InputManager.IsDownGamePad(Buttons.A))
					{
						// run a ray and get the closest collision
						var r = new Ray(obj.Position, ((BasicCamera)obj).GetCameraTarget());
						var rcp = scene.CollideRay(r);
						if (rcp.DidCollide)
							SpawnObject(new ModelObject("cubes/cube1").SetPosition(rcp.Position).SetScale(0.1f).AddScript(
								(s, o, t) =>
								{
									if (InputManager.IsKeyPressed(Keys.OemSemicolon)) s.RemoveObject(o);
								}));
					}

					if (InputManager.IsKeyPressed(Keys.N))
					{
						// run a ray and get the closest collision
						var r = new Ray(obj.Position, ((BasicCamera)obj).GetCameraTarget());
						var rcp = scene.CollideRay(r);
						if (rcp.DidCollide)
							this.RemoveObject(rcp.Collidable);
					}

					if (InputManager.IsKeyPressed(Keys.H))
					{
						this.SpawnObject(new ControllerVibration(0.2f, new StaticVibration(1,1)));
					}
				}), ChildProperties.Translation
			), ChildProperties.TranslationRotation));

			//AddObject(new ModelObject("slopes/slope1").SetPosition(-1.5f, -0.85f, -4));
			//AddObject(new ModelObject("slopes/slope2").SetPosition(-0.5f, -0.85f, -2));
			//AddObject(new ModelObject("slopes/slope3").SetPosition(0, -0.85f, 0));
			//AddObject(new ModelObject("slopes/slope4").SetPosition(0, -0.35f, 2));
			//AddObject(new ModelObject("slopes/slope5").SetPosition(0, 0.65f, 4));

			//AddObject(
			//	new FullCollidableModelObject("sphere")
			//		.SetCollision(new SphereCollision())
			//		.SetPosition(10, 10, 10)
			//		.AddScript((scene, go, time) => go.AddRotationAt(Axis.Y, -2, time))
			//		.AddScript((scene, go, time) => go.AddRotationAt(Axis.Z, -3, time))
			//		.AddScript((scene, go, time) => go.AddRotationAt(Axis.X, -5, time))
			//		.AddChild(
			//		new FullCollidableModelObject("sphere")
			//			.SetCollision(new SphereCollision())
			//			.SetPosition(2, 0, 2)
			//			.SetScale(1.5f)
			//			.AddScript((scene, go, time) =>
			//				{
			//					go.CustomData.SetValue("lifeTime", go.CustomData.GetValue<double>("lifeTime") + time.ElapsedGameTime.TotalSeconds);
			//					go.SetPositionAt(Axis.Y, (float)Math.Sin(go.CustomData.GetValue<double>("lifeTime")));
			//				})
			//			.AddChild(
			//				new FullCollidableModelObject("sphere")
			//					.SetCollision(new SphereCollision())
			//					.SetPosition(5, 0, 0)
			//					.AddScript((scene, go, time) => go.AddRotationAt(Axis.Y, 2, time))
			//					.AddScript((scene, go, time) => go.AddRotationAt(Axis.Z, 3, time))
			//					.AddScript((scene, go, time) => go.AddRotationAt(Axis.X, 5, time))
			//					.AddChild(
			//						new FullCollidableModelObject("sphere")
			//							.SetCollision(new SphereCollision())
			//							.SetPosition(3, 3, 0)
			//					, ChildProperties.TranslationRotation)
			//		, ChildProperties.Translation | ChildProperties.Scale)
			//	, ChildProperties.AllTransform)
			//);


			AddObject(new MappingTestObject().SetPosition(0, 30, 0));

			//AddObject(new TestBallObject().SetPosition(2, 3, 0));

			AddObject(new CollidableModelObject("cubes/cube5").SetPosition(0, -9, 0)
					//.SetRotation(0, 0, 0.4f)
					.SetCollision(new BoxCollision(new Vector3(-8), new Vector3(8))) //
			);

			//AddObject(new SpriteObject());
			AddObject(new GameObject().AddScript((scene, o, gameTime) =>
			{
				/* Close Game with Settings.ExitKey
				 * tempary Change ExitKey to P (didn't call SettingsManager.SaveSetting() so it will NOT be permanent, just for the time the application is running)
				 */
				if (InputManager.IsKeyPressed(Settings.ExitKey))
					SceneManager.CloseScene();
				if (InputManager.IsKeyPressed(Keys.I))
					SettingsManager.SetSetting("exitKey", Keys.P);

				if (InputManager.IsKeyPressed(Keys.NumPad1))
				{
					// create a plane that spans over the x axis

					// p: x = ZERO3 + (0, 1, 0) + (0, 0, 1)

					// create the plane
					var planeOrigin = new Vector3(0, 0, 0);
					var planeParameter1 = new Vector3(1, 0, 1);
					var planeParameter2 = new Vector3(0, 1, 1);
					var planeNormal = new Vector3(0, 1, 0); // pp1 x pp2

					// create our point of interest
					var sphere = new Vector3(2, 3, 5);

					// We want to solve Ax = b
					// create our Vector "b", which is our solution
					var b = sphere - planeOrigin;

					// now we can create a room with our plane and the normal which contains all points in R3
					var solution = VectorMathHelper.SolveLinearEquation(planeParameter1, planeParameter2, planeNormal, b);

					// now we should have a solution. (-1, 0, 0)

					Console.WriteLine(
						$"Solved {b} = {solution.X} * {planeParameter1} + {solution.Y} * {planeParameter2} + {solution.Z} * {planeNormal}");
					Console.WriteLine(
						$"Calculated: {solution.X * planeParameter1 + solution.Y * planeParameter2 + solution.Z * planeNormal}");
					//Console.WriteLine($"Solution: {solution}");
					//Console.WriteLine(planeOrigin + solution.X * planeParameter1 + solution.Y * planeParameter2 + solution.Z * planeNormal);
				}
			}));

			AddObject(new GameObject().SetPosition(0, 10, 0).AddScript(
				(scene, o, arg3) => o.AddRotation(0, 1, 0, arg3)
			).AddChild(new CollidableModelObject("sphere").SetPosition(0, 10, 0).SetCollision(new SphereCollision(1)), ChildProperties.TranslationRotation));


			this.AddObject(new GameObject().AddChild(new CollidableModelObject("sphere").SetPosition(10, 10, 0).SetCollision(new SphereCollision(1))));

			AddObject(new GameObject().AddScript((scene, obj, time) =>
			{
				if (!obj.CustomData.ContainsKey("timeAlive")) obj.CustomData.SetValue("timeAlive", 0.0f);

				obj.CustomData.SetValue("timeAlive",
					obj.CustomData.GetValue<float>("timeAlive") + (float) time.ElapsedGameTime.TotalSeconds);
			}));

			AddObject(new CollidableModelObject("cubes/cube1")
				.SetCollision(new BoxCollision(new Vector3(-0.5f), new Vector3(0.5f)))
				.SetScale(8.0f)
				.SetPosition(50, 0, 0)
			);

			// Add Child Test
			AddObject(new ParentBlock().SetPosition(20, -10, 0).SetDebugName("Parent"));
			//AddObject(new ChildBall().SetPosition(21, 00, 1).SetDebugName("Child"));
			//AddObject(new ChildBall().SetPosition(21, 00, -1).SetDebugName("Child"));
			//AddObject(new ChildBall().SetPosition(19, 00, 1).SetDebugName("Child"));
			//AddObject(new ChildBall().SetPosition(19, 00, -1).SetDebugName("Child"));

			AddObject(new GameObject().AddScript(((scene, o, time) =>
			{
				if (InputManager.IsKeyPressed(Keys.NumPad9))
				{
					o.CustomData.SetValue("millisToSleep", o.CustomData.GetValue<int>("millisToSleep") + 1);
					// REPLACE THIS WITH WHATEVER SYSTEM YOU USE FOR NOTIFICATIONS: 
					//MessageBoxUtil.Notify("SLEEPING " + millisToSleep + " PER FRAME");
				}
				else if (InputManager.IsKeyPressed(Keys.NumPad8))
				{
					o.CustomData.SetValue("millisToSleep", o.CustomData.GetValue<int>("millisToSleep") - 1);
					//MessageBoxUtil.Notify("SLEEPING " + millisToSleep + " PER FRAME");
				}
				if (o.CustomData.GetValue<int>("millisToSleep") > 0)
				{
					System.Threading.Thread.Sleep(o.CustomData.GetValue<int>("millisToSleep"));
				}
				else
				{
					o.CustomData.SetValue("millisToSleep", 0);
				}
				//Console.WriteLine($"Sleeping for {o.CustomData.GetValue<int>("millisToSleep")} ms");

			})));

			SoundManager.RegisterSoundFromFile("LoopA", "Content/sound/Into The Wide World (OST 1) Loop A.wav");
			SoundManager.RegisterSoundFromFile("LoopB", "Content/sound/Into The Wide World (OST 1) Loop B.wav");
			SoundManager.RegisterSoundFromFile("LoopAFilter", "Content/sound/Into The Wide World (OST 1) Loop A Filter.wav");
			SoundManager.RegisterSoundFromFile("LoopBFilter", "Content/sound/Into The Wide World (OST 1) Loop B Filter.wav");
			SoundManager.RegisterSoundFromFile("Jump", "Content/sound/Jump1.wav");
			AddObject(new GameObject().AddScript((scene, o, arg3) =>
			{
				if(InputManager.IsKeyPressed(Keys.M))
					SoundManager.PlayLoopMusic("LoopA", "LoopB");
				if(InputManager.IsKeyPressed(Keys.N))
					SoundManager.PlayEffect("Jump");

				if(InputManager.IsKeyPressed(Keys.F9))
					SoundManager.PlayDualLoopMusic("LoopA", "LoopB", "LoopAFilter", "LoopBFilter");
				if(InputManager.IsKeyPressed(Keys.F10))
					SoundManager.TogglePrimarySecondary();
			}));

			AddObject(new MovingBlock("cubes/cube1", 2.0f, new Vector3(-20, 0, 20), new Vector3(20, 0, 20), new Vector3(20, 0, -20), new Vector3(-20, 0, -20))
			.SetCollision(new BoxCollision(-0.5f, 0.5f)));
			

		}

		//public override void AddLightningToEffect(Effect eff)
		//{
		//	//var effect = (BasicEffect) eff;

		//	//effect.DirectionalLight0.DiffuseColor = new Vector3(0.15f, 0.15f, 0.15f); // some diffuse light
		//	//effect.DirectionalLight0.Direction = new Vector3(1, -2, 1);  // 
		//	//effect.DirectionalLight0.SpecularColor = new Vector3(0.05f, 0.05f, 0.05f); // a tad of specularity]
		//	////effect.AmbientLightColor = new Vector3(1f, 1f, 1f); // Add some overall ambient light.
		//	//effect.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.315f); // Add some overall ambient light.
		//}
	}
}