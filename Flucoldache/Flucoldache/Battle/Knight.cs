using Microsoft.Xna.Framework;

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
			MinAttack = 17;
			MaxAttack = 20;
			Defence = 30;
			Speed = 5;
			ForegroundColor = GameConsole.BaseBackgroundColor;
			BackgroundColor = Color.LightGray;

			PreserveBackground = true;
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
