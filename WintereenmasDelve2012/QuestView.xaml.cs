﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game.quests.maps;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.quests.maps;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game.quests;
using System.Reflection;
using System.IO;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.storyTelling;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.chance;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.turnStepAction;
using System.Windows.Threading;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.quests.maps.tileActions;

namespace WintereenmasDelve2012
{
	/// <summary>
	/// Interaction logic for QuestView.xaml
	/// </summary>
	public partial class QuestView : Page
	{
		private const int MAP_TILE_SIZE_IN_PIXELS = 50;

		private AbstractQuest _quest;
		private ChanceProvider _chanceProvider;

		private StoryTeller _storyTeller;

		public EventHandler HeroesWin;
		public EventHandler HeroesLose;

		private Avatar _currentTurnTaker;

		private QuestAnalyzer _questAnalyzer;
		private DispatcherTimer _turnTimer;

		public QuestView(AbstractQuest quest, ChanceProvider chanceProvider, StoryTeller storyTeller)
		{
			InitializeComponent();

			_quest = quest;
			_chanceProvider = chanceProvider;
			_storyTeller = storyTeller;
			
			_currentTurnTaker = null;

			_turnTimer = new DispatcherTimer();
			_turnTimer.Interval = TimeSpan.FromMilliseconds(500);
			_turnTimer.Tick += OnTurnTimerTick;

			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, EventArgs args)
		{
			Loaded -= OnLoaded;

			// Render the quest board
			RenderQuestBoard();

			// Read the quest description
			_storyTeller.StoryComplete += OnQuestIntroductionStoryComplete;
			_storyTeller.TellStory(_quest.IntroductionStory);
		}

		private void RenderQuestBoard()
		{
			xamlQuestMapBoard.Children.Clear();

			int tilesWide = _quest.Map.TilesWide;
			int tilesTall = _quest.Map.TilesTall;
			for (int y = 0; y < tilesTall; y++)
			{
				for (int x = 0; x < tilesWide; x++)
				{
					List<MapTile> mapTiles = _quest.Map.GetMapTilesForLocation(x, y);
					for(int i=0; i<mapTiles.Count; i++)
					{
						MapTile mapTile = mapTiles[i];
						Image tileImage = new Image();
						tileImage.Source = GetImageSourceForPath(mapTile.ImagePath);
						tileImage.MaxHeight = MAP_TILE_SIZE_IN_PIXELS;
						tileImage.MaxWidth = MAP_TILE_SIZE_IN_PIXELS;
						xamlQuestMapBoard.Children.Add(tileImage);
						Canvas.SetLeft(tileImage, x * MAP_TILE_SIZE_IN_PIXELS);
						Canvas.SetTop(tileImage, y * MAP_TILE_SIZE_IN_PIXELS);
					}
				}
			}

			xamlQuestMapBoard.Width = tilesWide * MAP_TILE_SIZE_IN_PIXELS;
			xamlQuestMapBoard.Height = tilesTall * MAP_TILE_SIZE_IN_PIXELS;
		}

		private ImageSource GetImageSourceForPath(String imagePath)
		{
			BitmapImage logo = new BitmapImage();
			logo.BeginInit();
			logo.UriSource = new Uri(imagePath, UriKind.Relative);
			logo.EndInit();
			return logo;
		}

		private void OnQuestIntroductionStoryComplete(object sender, EventArgs args)
		{ BeginTurnCycle(); }

		private void BeginTurnCycle()
		{
			_storyTeller.StoryComplete -= OnQuestIntroductionStoryComplete;

			List<Avatar> turnTakers = _quest.TurnTakers;

			// Make the first person in the list the current player
			_currentTurnTaker = turnTakers[0];
			_currentTurnTaker.StartTurn();

			_questAnalyzer = new QuestAnalyzer(_quest);
			PerformNextTurnCycle(); // Immediately take one turn
		}

		private void OnTurnTimerTick(object sender, EventArgs args)
		{ PerformNextTurnCycle(); }

		private void PerformNextTurnCycle()
		{
			_turnTimer.Stop();

			// Check to make sure there is a living hero left.  If not, stop the timer, and end the game.  
			// This has to be above all of the turn loops to make sure that the "Hero is dead" action comes up for each one.
			if (!_quest.AreAnyHeroesAlive)
			{
				if (HeroesLose != null)
				{ HeroesLose(this, new EventArgs()); }
			}
			else
			{
				TurnStepAction action = _currentTurnTaker.DoTakeTurnStep(_questAnalyzer, _chanceProvider);

				if (action == null) // The current player has no more steps to take
				{
					// Get the current turn takers' index

					List<Avatar> turnTakers = _quest.TurnTakers;
					int currentIndex = turnTakers.IndexOf(_currentTurnTaker);

					currentIndex++;
					if (currentIndex > turnTakers.Count - 1)
					{ currentIndex = 0; }
					_currentTurnTaker = turnTakers[currentIndex];
					_currentTurnTaker.StartTurn();
					PerformNextTurnCycle();
				}
				else
				{
					action.Complete += OnCurrentActionCommitComplete;
					action.Commit(_quest, _storyTeller, _chanceProvider);
				}

				RenderQuestBoard();
			}
		}

		private void OnCurrentActionCommitComplete(object sender, EventArgs args)
		{
			if (sender is TurnStepAction)
			{
				TurnStepAction action = (TurnStepAction) sender;
				action.Complete -= OnCurrentActionCommitComplete;
			}
			_turnTimer.Start(); // Start the timer for the next turn
		}
	}
}
