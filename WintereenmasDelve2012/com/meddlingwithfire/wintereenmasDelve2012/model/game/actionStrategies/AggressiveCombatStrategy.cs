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
			List<Avatar> enemies = mapAnalyzer.GetAdjacentEnemies(currentAvatar, currentAvatar.CanAttackAdjacent);

			if (enemies.Count <= 0)
			{ return null; }

			// TODO: Determine which enemy to attack

			// Attack enemy action
			AttackAction action = new AttackAction(currentAvatar, enemies[0]);

			return action;
		}
	}
}
