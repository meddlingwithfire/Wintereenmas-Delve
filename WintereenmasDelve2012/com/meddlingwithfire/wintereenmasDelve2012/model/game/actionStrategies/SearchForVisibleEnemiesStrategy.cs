using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.turnStepAction;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.chance;

namespace WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.actionStrategies
{
	public class SearchForVisibleEnemiesStrategy : AbstractActionStrategy
	{
		public SearchForVisibleEnemiesStrategy()
			: base(true)
		{

		}

		override public TurnStepAction FindAction(Avatar currentAvatar, AvatarTurnState avatarTurnState, QuestAnalyzer mapAnalyzer, ChanceProvider chanceProvider)
		{
			List<Avatar> visibleEnemies = mapAnalyzer.GetAdjacentEnemies(currentAvatar, currentAvatar.CanAttackAdjacent);


			return null;
		}
	}
}
