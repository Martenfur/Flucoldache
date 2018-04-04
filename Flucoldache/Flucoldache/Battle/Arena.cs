using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Monofoxe.Engine.Drawing;
using System.Xml;
using System.Diagnostics;
using Microsoft.Xna.Framework;

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


		public Arena(string fileName)
		{
			EnemyTypes = new Dictionary<string, Type>();
			EnemyTypes.Add("enemy", Type.GetType("Flucoldache.Battle.Enemy"));

			LoadArena(_rootDir + fileName);
		}

		public void LoadArena(string arenaFile)
		{
			XmlDocument xml = new XmlDocument();
			xml.Load(arenaFile);
			
			XmlNodeList nodes = xml.DocumentElement.ChildNodes;//xml.DocumentElement.SelectNodes("objects");
			
			foreach(XmlNode node in nodes.Item(0).SelectNodes("object"))
			{
				Vector2 pos = new Vector2(Int32.Parse(node.Attributes["x"].Value), Int32.Parse(node.Attributes["y"].Value));
				//Debug.WriteLine(pos + " " + node.Attributes["classname"].Value);
			}
				
			//nodes = xml.DocumentElement.SelectNodes("enemies");
			
			foreach(XmlNode node in nodes.Item(1).SelectNodes("enemy"))
			{
			//	Debug.WriteLine(node.Attributes["type"].Value);
			}

			CreateFormation(47);
		}

		/// <summary>
		/// Puts enemies into the formation.
		/// </summary>
		/// <param name="objCount"></param>
		public void CreateFormation(int count)
		{
			int objCount = count;

			int columnCount = (int)Math.Ceiling(objCount / (float)_formationH);
			
			string[] columns = new string[_formationH];

			for(var x = 0; x < columnCount - 1; x += 1)
			{
				for(var y = 0; y < _formationH; y += 1)
				{
					columns[y] += 'a';
					objCount -= 1;
				}	
			}

			// Last row.
			Debug.WriteLine(objCount);
			if (objCount % 2 == 0)
			{
				int lastColumnCount = objCount / 2 + 1;
				for(var y = 0; y < _formationH / 2; y += 1)
				{
					columns[y * 2 + 1] += 'a';
					objCount -= 1;
				}

				var i = 0;
				while(objCount > 0)
				{
					columns[i * 2] += 'a';
					columns[(_formationH - 1) - (i * 2)] += 'a';
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
					columns[center + i] += 'a';

					if (i > 0)
					{
						columns[center - i] += 'a';
						objCount -= 1;
					}
					
					i += 1;
					objCount -= 1;
				}
			}
			// Last row.

			foreach(string row in columns)
			{
				Debug.WriteLine(row);
			}

		}

	}
}
