using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Flucoldache.Overworld;
using Monofoxe.Engine.Drawing;
using System.IO;

namespace Flucoldache
{

	/// <summary>
	/// Main gameplay controller.
	/// </summary>
	public class GameplayController : GameObj
	{
		public static Random Random = new Random(DateTime.Now.Millisecond);
	
		public static string SaveDir = "Save";	

		public GameplayController()
		{
			GameConsole.Init(Fonts.Font, 96, 32);

			GameConsole.Camera.BackgroundColor = Color.Black;

			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None; 
			rasterizerState.ScissorTestEnable = false;
			rasterizerState.FillMode = FillMode.Solid;
			DrawCntrl.Rasterizer = rasterizerState;
			DrawCntrl.Sampler = SamplerState.PointClamp;
			
			//new Terrain(96, 32);
			//new MapEditor();
			new MainMenu();
		}

		public override void Update()
		{
			
		}

		public static void SaveGame()
		{
			
			string path = "/" + SaveDir;
			string fullPath = Environment.CurrentDirectory + path;

			if (Directory.Exists(fullPath))
			{
				DirectoryInfo di = new DirectoryInfo(fullPath);

				foreach(FileInfo file in di.GetFiles())
				{
					file.Delete(); 
				}
			}
			else
			{
				Directory.CreateDirectory(fullPath);
			}

			var i = 0;
			foreach(LootContainer container in Objects.GetList<LootContainer>())
			{
				container.GenerateLootTable(path, i);
				i += 1;
			}

			foreach(Terrain terrain in Objects.GetList<Terrain>())
			{
				MapEditor.SaveMap(terrain, fullPath + "/save.map");
				break;
			}

			foreach(Inventory inventory in Objects.GetList<Inventory>())
			{
				inventory.SaveInventory(path);
			}

		}

		public static void LoadGame()
		{
			string path = "/" + SaveDir;
			string fullPath = Environment.CurrentDirectory + path;

			MapEditor.LoadMap(fullPath + "/save.map", false);
			
			foreach(Inventory inventory in Objects.GetList<Inventory>())
			{
				Objects.Destroy(inventory);
			}

			Inventory inv = new Inventory();

			inv.LoadInventory(fullPath);
		}

	}
}
