using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using System.Diagnostics;

namespace Flucoldache.Battle
{
	public class Enemy : ArenaObj
	{
		Dialogue _attackDialogue;
		Dialogue _deathDialogue;

		bool _skipTurn;
		bool _rabies;

		public override void Update()
		{
			base.Update();

			if (Health <= 0 && !Initiative)
			{
				Objects.Destroy(this);
			}
		}

		/// <summary>
		/// Returns the most left unit in the formation.
		/// </summary>
		public Enemy GetMostLeftUnit()
		{
			Arena arena = (Arena)Objects.ObjFind<Arena>(0);
			
			int x = (int)Pos.X;

			List<Enemy> enemies = Objects.GetList<Enemy>();

			Enemy leftEnemy = this;

			while(x >= arena.FormationPos.X)
			{
				x -= (int)Arena.UnitSpacing.X + 1;

				foreach(Enemy enemy in enemies)
				{
					if (enemy.Pos.X == x && enemy.Pos.Y == Pos.Y)
					{
						leftEnemy = enemy;
					}
				}
			}

			return leftEnemy;
		}

		public override void ReceiveInitiative()
		{
			base.ReceiveInitiative();

			foreach(StatEffect effect in Effects)
			{
				if (effect.Proc)
				{
					if (effect.Token == "flu")
					{
						_skipTurn = true;
					}

					if (effect.Token == "rabies")
					{
						_rabies = true;
					}
				}
			}

			if (!Waiting && !_rabies)
			{
				Enemy enemy = GetMostLeftUnit();

				if (enemy != this)
				{
					Arena arena = (Arena)Objects.ObjFind<Arena>(0);
					arena.GiveInitiative();
				}
			}
		}

		protected void PerformBasicAttack()
		{
			Enemy enemy = GetMostLeftUnit();

			if ((enemy != this && !_rabies) || Health <= 0)
			{
				Arena arena = (Arena)Objects.ObjFind<Arena>(0);
				arena.GiveInitiative();
				return;
			}

			if (_attackDialogue == null)
			{
				if (_skipTurn)
				{
					_attackDialogue = new Dialogue(new string[]{""}, new string[]{Name + " чувствует себя очень плохо и пропускает свой ход."});
					_skipTurn = false;
				}
				else
				{
					if (_rabies)
					{
						int objId = (int)(GameplayController.Random.NextDouble() * (float)Objects.Count<ArenaObj>());
						Debug.WriteLine(objId);
						ArenaObj obj = (ArenaObj)Objects.ObjFind<ArenaObj>(objId);
						
						int dmg = Attack(obj);
						
						_attackDialogue = new Dialogue(new string[]{""}, 
						new string[]{"В приступе бешенства " + Name.ToLower() + " атакует " + obj.Name1.ToLower() + " и наносит " + dmg + " урона."});
					}
					else
					{
						int dmg = Attack((ArenaPlayer)Objects.ObjFind<ArenaPlayer>(0));
					
						_attackDialogue = new Dialogue(new string[]{""}, new string[]{Name + " атакует и наносит " + dmg + " урона."});
					}
				}
			}
			else
			{
				if (_attackDialogue.Destroyed)
				{
					_attackDialogue = null;
					Arena arena = (Arena)Objects.ObjFind<Arena>(0);
					arena.GiveInitiative();
					_rabies = false;
				}
			}
		}
	}
}
