using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class Knight : Enemy
	{
		public Knight()
		{
			Name = Strings.KnightName1;
			Name1 = Strings.KnightName2;

			MaxHealth = 60;
			Health = MaxHealth;
			MinAttack = 15;
			MaxAttack = 25;
			Defence = 30;
			Speed = 5;
			ForegroundColor = Color.Black;
			BackgroundColor = Color.LightGray;
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
			
			GameConsole.DrawChar('T', Pos);
		}



	}
}
