using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game.quests.maps;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.storyTelling;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.quests.maps;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model;

namespace WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game.quests
{
	public class AbstractQuest
	{
		public Story IntroductionStory;

		public QuestMap Map;

		public List<Avatar> Heroes;
		public List<Avatar> Monsters;

		private Dictionary<Avatar, MapTile> _avatarMapTiles;
		
		public AbstractQuest(Point mapStaircaseOrigin, List<Avatar> heroes)
			: base()
		{
			_avatarMapTiles = new Dictionary<Avatar, MapTile>();
			Map = new QuestMap(mapStaircaseOrigin);

			Heroes = heroes;

			// Add the Hero map tiles to the quest board
			List<Point> staircaseLocations = Map.HeroStartingLocations;
			int counter = 0;
			foreach(Hero hero in Heroes)
			{
				int index = counter % staircaseLocations.Count;
				Point location = staircaseLocations[index];
				MapTile heroTile = new MapTile(hero.ImagePath, false, false, false, false, false);
				_avatarMapTiles[hero] = heroTile;
				Map.AddTile(location, heroTile);
				counter++;
			}

			Monsters = new List<Avatar>();
		}

		public Point GetAvatarLocation(Avatar avatar)
		{
			if (!_avatarMapTiles.ContainsKey(avatar))
			{ throw new Exception("AbstractQuest GetAvatarLocation(avatar) avatar is not in the Quest!"); }

			Point point = Map.GetMapTileLocation(_avatarMapTiles[avatar]);
			if (point == null)
			{ throw new Exception("AbstractQuest GetAvatarLocation(avatar) Unable to find a location for the avatar's map tile!"); }

			return point;
		}

		public MapTile GetAvatarMapTile(Avatar avatar)
		{
			if (!_avatarMapTiles.ContainsKey(avatar))
			{ throw new Exception("AbstractQuest GetAvatarLocation(avatar) avatar is not in the Quest!"); }

			return _avatarMapTiles[avatar];
		}

		public Boolean AreAnyHeroesAlive
		{
			get
			{
				foreach (Hero hero in Heroes)
				{
					if (hero.IsAlive)
					{ return true; }
				}
				return false;
			}
		}

		public List<Avatar> TurnTakers
		{
			get
			{
				List<Avatar> turnTakers = new List<Avatar>();
				for (int i = 0; i < Heroes.Count; i++)
				{ turnTakers.Add(Heroes[i]); }
				for (int i = 0; i < Monsters.Count; i++)
				{ turnTakers.Add(Monsters[i]); }
				return turnTakers;
			}
		}

		public void RemoveAvatar(Avatar avatar)
		{
			if (Heroes.Contains(avatar))
			{ Heroes.Remove(avatar); }
			if (Monsters.Contains(avatar))
			{ Monsters.Remove(avatar); }

			// Remove their map tile
			MapTile tile = _avatarMapTiles[avatar];
			Map.RemoveMapTile(tile);
		}

		public List<Avatar> GetEnemiesOfAvatar(Avatar avatar)
		{
			List<Avatar> enemies = new List<Avatar>();

			switch (avatar.Faction)
			{
				case Faction.Heroes:
					foreach (Monster monster in Monsters)
					{ enemies.Add(monster); }
					break;
				case Faction.Xargon:
					foreach (Hero hero in Heroes)
					{ enemies.Add(hero); }
					break;
			}

			return enemies;
		}

		public Dictionary<Avatar, MapTile> AvatarMapTiles
		{
			get { return _avatarMapTiles; }
		}

		public void AddMonster(Monster monster, Point toLocation)
		{
			Monsters.Add(monster);
			MapTile monsterTile = new MapTile(monster.ImagePath, false, false, false, false, false);
			_avatarMapTiles[monster] = monsterTile;
			Map.AddTile(toLocation, monsterTile);
		}
	}
}
