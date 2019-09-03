using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Flucoldache.Overworld;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.FMODAudio;

namespace Flucoldache.Battle
{
	class Arena : GameObj
	{
		public Dictionary<string, Type> EnemyTypes;

		static string _rootDir = "Resources/Arenas/";

		/// <summary>
		/// Height of the enemy formation.
		/// MUST be uneven!
		/// </summary>
		static int _formationH = 5;

		public static Vector2 UnitSpacing = new Vector2(2, 1);

		
		public List<ArenaObj> Units;
		public List<ArenaObj> UnitTurnOrderList;
		public int CurrentUnit = 0;

		Vector2 _center;
		int _spacing = 8; // Space between player and other units.
		
		public Vector2 FormationPos;
		public Vector2 FormationSize;
		Vector2 _playerPos;
		int _formationAlignY;

		bool _win = false;
		Dialogue _winDialogue;
		bool _lose = false;
		Dialogue _loseDialogue;

		ArenaBackground _bkg;

		Sound WinSound;

		bool _isFinalBattle = false;

		public Arena(string fileName)
		{
			if (fileName.Contains("final"))
			{
				SoundController.PlaySong(SoundController.FinalBattle);
				_isFinalBattle = true;
			}
			else
			{
				SoundController.PlaySong(SoundController.Battle);
			}

			_bkg = new ArenaBackground();
				
			DrawCntrl.Cameras[0].X = 0;
			DrawCntrl.Cameras[0].Y = 0;

			Depth = 9000;

			foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
			{
				obj.Active = false;
			}
			foreach(Terrain obj in Objects.GetList<Terrain>())
			{
				obj.Active = false;
			}


			_center = new Vector2(GameConsole.W / 2 - 10, (GameConsole.H - Dialogue.Size.Y) / 2);
			FormationPos = _center + new Vector2(_spacing, 0);
			_playerPos = _center + new Vector2(-_spacing + 1, 0);

			EnemyTypes = new Dictionary<string, Type>();
			EnemyTypes.Add("dummy", Type.GetType("Flucoldache.Battle.DummyEnemy"));
			EnemyTypes.Add("villager", Type.GetType("Flucoldache.Battle.Villager"));
			EnemyTypes.Add("wolf", Type.GetType("Flucoldache.Battle.Wolf"));
			EnemyTypes.Add("bear", Type.GetType("Flucoldache.Battle.Bear"));
			EnemyTypes.Add("rabbit", Type.GetType("Flucoldache.Battle.Rabbit"));
			EnemyTypes.Add("knight", Type.GetType("Flucoldache.Battle.Knight"));

			LoadArena(_rootDir + fileName);

			
			UpdateUnitSpeedList();
			UnitTurnOrderList[CurrentUnit].Initiative = true;
		}

		bool _blackscreenActivated = false;
		float _blackscreenAlpha = 0;
		float _blackscreenSpeed = 0.3f;

		public override void Update()
		{
			base.Update();
			if (_blackscreenActivated)
			{
				_blackscreenAlpha += (float)GameCntrl.Time(_blackscreenSpeed);
			}

			if (Objects.GetList<Dialogue>().Count == 0 && Units.Count == 1 && Units[0] is ArenaPlayer)
			{
				_win = true;
			}

			if (CurrentUnit >= UnitTurnOrderList.Count || UnitTurnOrderList[CurrentUnit].Destroyed)
			{
				//CurrentUnit = 0;
				Units[0].Initiative = true;
				
				Units[0].ReceiveInitiative();
			}
			//Console.WriteLine(CurrentUnit + " " + UnitTurnOrderList[CurrentUnit].Waiting);

		}

		public override void UpdateEnd()
		{
			Units.RemoveAll(o => o.Destroyed);
			/*
			if (Input.KeyboardCheckPress(Microsoft.Xna.Framework.Input.Keys.W))
			{
				// TODO: Remove!
				_win = true;
			}

			if (Input.KeyboardCheckPress(Microsoft.Xna.Framework.Input.Keys.L))
			{
				// TODO: Remove!
				_lose = true;
			}*/


			if (_win)
			{
				UnitTurnOrderList[CurrentUnit].Initiative = false;
				if (_winDialogue == null)
				{
					SoundController.CurrentSong.Stop();
					WinSound = SoundController.PlaySound(SoundController.Win);

					_winDialogue = new Dialogue(new string[]{""}, new string[]{Strings.BattleWin});
					_winDialogue.Locked = true;
				}
				else
				{
					if (!WinSound.IsPlaying)
					{
						Objects.Destroy(_winDialogue);
						Objects.Destroy(this);
					}
				}
			}

			ArenaPlayer player = (ArenaPlayer)Objects.ObjFind<ArenaPlayer>(0);
			
			if (_lose)
			{
				_blackscreenActivated = true;
				SoundController.CurrentSong.Volume = 1 - Math.Max(0, Math.Min(1, _blackscreenAlpha));

				if (_loseDialogue == null)
				{
					_loseDialogue = new Dialogue(new string[]{""}, new string[]{Strings.BattleDefeat});
				}
				else
				{
					if (_loseDialogue.Destroyed)
					{
						Objects.Destroy(this);
						GameplayController.LoadGame();
					}
				}
			}

		}

		public override void Destroy()
		{
			base.Destroy();

			Objects.Destroy(_bkg);
			Controls.Enabled = true;

			if (_isFinalBattle)
			{
				new Endgame();
			}
			else
			{
				foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
				{
					obj.Active = true;
				}
				foreach(Terrain obj in Objects.GetList<Terrain>())
				{
					obj.Active = true;
				}
			}
			SoundController.PlaySong(SoundController.Overworld);

			foreach(ArenaObj obj in Objects.GetList<ArenaObj>())
			{
				Objects.Destroy(obj);
			}						
			foreach(Dialogue obj in Objects.GetList<Dialogue>())
			{
				Objects.Destroy(obj);
			}
			foreach(SelectionMenu obj in Objects.GetList<SelectionMenu>())
			{
				Objects.Destroy(obj);
			}
		}

		public override void Draw()
		{
			GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;
			GameConsole.ForegroundColor = GameConsole.BaseForegroundColor;
			GameConsole.DrawFrame(Dialogue.Pos - Vector2.One, Dialogue.Size + Vector2.One * 2);
			GameConsole.DrawRectangle(Dialogue.Pos, Dialogue.Size);
		}

		public override void DrawEnd()
		{
			ArenaPlayer player = (ArenaPlayer)Objects.ObjFind<ArenaPlayer>(0);
			
			if (player != null)
			{
				GameConsole.ForegroundColor = GameConsole.BaseForegroundColor;
				GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;
				
				GameConsole.DrawText("HP: " + player.Health.ToString().PadLeft(3) + "/" + player.MaxHealth, Dialogue.Pos - Vector2.UnitY * 1);
				
				GameConsole.ForegroundColor = GameConsole.HealthForegroundColor;
				GameConsole.BackgroundColor = GameConsole.HealthBackgroundColor;
				GameConsole.DrawRectangle((int)Dialogue.Pos.X + 12, (int)Dialogue.Pos.Y - 1, 16, 1);
				GameConsole.DrawProgressBar((int)Dialogue.Pos.X + 12, (int)Dialogue.Pos.Y - 1, 16, ((float)player.Health) / ((float)player.MaxHealth));
			}

			GameConsole.BackgroundColor = new Color(GameConsole.BaseBackgroundColor, _blackscreenAlpha);
			GameConsole.DrawRectangle(0, 0, GameConsole.W, GameConsole.H);
		}

		public void GiveInitiative()
		{
			UnitTurnOrderList[CurrentUnit].Initiative = false;
			if (Units.Count == 1 && Units[0] is ArenaPlayer)
			{
				_win = true;
				return;
			}

			ArenaPlayer player = (ArenaPlayer)Objects.ObjFind<ArenaPlayer>(0);
			if (player.Health <= 0)
			{
				_lose = true;
				return;
			}

			do
			{
				CurrentUnit += 1;
			}
			while(CurrentUnit < UnitTurnOrderList.Count && UnitTurnOrderList[CurrentUnit].Destroyed);

			if (CurrentUnit >= UnitTurnOrderList.Count)
			{
				// New turn.
				CurrentUnit = 0;
				UpdateUnitSpeedList();
				foreach(ArenaObj obj in Units)
				{
					foreach(StatEffect effect in obj.Effects)
					{
						effect.Infect();
					}
				}
				// New turn.
			}
			
			UnitTurnOrderList[CurrentUnit].ReceiveInitiative();
		}

		void UpdateUnitSpeedList()
		{
			UnitTurnOrderList = Units.OrderByDescending(o => o.Speed).ToList();
		}

		public void LoadArena(string arenaFile)
		{
			XmlDocument xml = new XmlDocument();
			xml.Load(arenaFile);
			
			XmlNodeList nodes = xml.DocumentElement.ChildNodes;
			
			foreach(XmlNode node in nodes.Item(0).SelectNodes("object"))
			{
				Vector2 pos = new Vector2(Int32.Parse(node.Attributes["x"].Value), Int32.Parse(node.Attributes["y"].Value));
			}
			
			Units = new List<ArenaObj>();

			foreach(XmlNode node in nodes.Item(1).SelectNodes("enemy"))
			{
				ArenaObj enemy = (ArenaObj)Activator.CreateInstance(EnemyTypes[node.Attributes["type"].Value]);
				Units.Add(enemy);
			}


			int[,] formation = CreateFormation(Units.Count);

			FormationSize = new Vector2(formation.GetLength(0), formation.GetLength(1)) * (UnitSpacing + Vector2.One) - UnitSpacing;

			int enemyId = 0;
			_formationAlignY = formation.GetLength(1) * ((int)UnitSpacing.Y + 1) / 2 - 1;
			for(var x = 0; x < formation.GetLength(0); x += 1)
			{
				for(var y = 0; y < formation.GetLength(1); y += 1)
				{
					if (formation[x, y] == 1)
					{
						Units[enemyId].Pos = FormationPos + new Vector2(x, y) * (UnitSpacing + Vector2.One) - new Vector2(0, _formationAlignY);
						enemyId += 1;
					}
				}
			}

			ArenaPlayer player = new ArenaPlayer();
			player.Pos = _playerPos;

			Units.Add(player);

		}

		/// <summary>
		/// Puts enemies into the formation.
		/// </summary>
		/// <param name="objCount"></param>
		public int[,] CreateFormation(int count)
		{
			int objCount = count;

			int columnCount = (int)Math.Ceiling(objCount / (float)_formationH);
			
			int[,] formation = new int[columnCount, _formationH];

			for(var x = 1; x < columnCount; x += 1)
			{
				for(var y = 0; y < _formationH; y += 1)
				{
					formation[x, y] = 1;
					objCount -= 1;
				}	
			}

			// Last row.

			int lastColumn = 0;

			if (objCount % 2 == 0)
			{
				int lastColumnCount = objCount / 2 + 1;
				for(var y = 0; y < _formationH / 2; y += 1)
				{
					formation[lastColumn, y * 2 + 1] = 1;
					objCount -= 1;
				}

				var i = 0;
				while(objCount > 0)
				{
					formation[lastColumn, i * 2] = 1;
					formation[lastColumn, (_formationH - 1) - (i * 2)] = 1;
					i += 1;
					objCount -= 2;
				}

			}
			else
			{
				/*
				For uneven counts.
				Arranges objects from center outwards in both directions.
				*/
				var center = _formationH / 2;
				var i = 0;
				while(objCount > 0)
				{
					formation[lastColumn, center + i] = 1;

					if (i > 0)
					{
						formation[lastColumn, center - i] = 1;
						objCount -= 1;
					}
					
					i += 1;
					objCount -= 1;
				}
			}
			// Last row.

			return formation;

		}

		public bool IsInFormationBounds(Vector2 pos)
		{
			return (pos.X >= FormationPos.X 
				&& pos.Y >= FormationPos.Y - _formationAlignY
				&& pos.X < FormationPos.X + FormationSize.X
				&& pos.Y < FormationPos.Y + FormationSize.Y - _formationAlignY
			);
		}

	}
}
