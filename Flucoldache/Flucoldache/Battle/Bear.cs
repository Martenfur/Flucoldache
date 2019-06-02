using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class Bear : Enemy
	{
		public Bear()
		{
			Name = Strings.BearName1;
			Name1 = Strings.BearName2;

			MaxHealth = 75;
			Health = MaxHealth;
			MinAttack = 30;
			MaxAttack = 40;
			Defence = 3;
			Speed = 5;
			ForegroundColor = Color.Brown;
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
			
			GameConsole.DrawChar('B', Pos);
		}



	}
}
