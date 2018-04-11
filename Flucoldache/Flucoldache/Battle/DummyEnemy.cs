using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class DummyEnemy : Enemy
	{
		public DummyEnemy()
		{
			Name = "Чучело";
			Name1 = "Чучело";

			MaxHealth = 30;
			Health = MaxHealth;
			MinAttack = 10;
			MaxAttack = 20;
			Defence = 2;
			Speed = 1;
		}

		

		public override void Update()
		{
			base.Update();
			
			if (Initiative && !Waiting)
			{
				PerformBasicAttack();
			}
		}

		public override void Draw()
		{
			base.Draw();
			
			GameConsole.DrawChar('D', Pos);
		}



	}
}
