using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.turnStepAction;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.chance;

namespace WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.actionStrategies
{
	public class AggressiveCombatStrategy : AbstractActionStrategy
	{
		/// <summary>
		/// Attacks any enemy within swinging range.
		/// </summary>
		public AggressiveCombatStrategy()
			: base(true)
		{ }

		override public TurnStepAction FindAction(Avatar currentAvatar, AvatarTurnState avatarTurnState, QuestAnalyzer mapAnalyzer, ChanceProvider chanceProvider)
		{
			// Attack enemy action
			List<Avatar> enemies = mapAnalyzer.GetAdjacentEnemies(currentAvatar, currentAvatar.CanAttackAdjacent);
			if (enemies.Count > 0)
			{
				// TODO: Determine which enemy to attack
				return new AggressiveAttackAction(currentAvatar, enemies[0]);
			}
			
			// Search for any visible enemies
			enemies = mapAnalyzer.GetVisibleEnemies(currentAvatar);
			if (enemies.Count <= 0)
			{ return null; } // If there are no visible enemies, then this strategy passes.
			
			// TODO: Determine which enemy to attack
			PointList path = mapAnalyzer.GetPathToEnemy(currentAvatar, enemies[0]);

			MovementTurnStepAction action = new MovementTurnStepAction(currentAvatar, path);
			return action;
		}
	}
}
