using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class Enemy : ArenaObj
	{
		public override void Update()
		{
			base.Update();

			if (Health <= 0)
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

			Enemy enemy = GetMostLeftUnit();

			if (enemy != this)
			{
				Arena arena = (Arena)Objects.ObjFind<Arena>(0);
				arena.GiveInitiative();
			}
		}
	}
}
