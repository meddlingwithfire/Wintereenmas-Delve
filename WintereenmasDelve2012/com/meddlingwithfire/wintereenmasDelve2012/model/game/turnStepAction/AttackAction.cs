using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.game.quests;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.storyTelling;
using WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.chance;

namespace WintereenmasDelve2012.com.meddlingwithfire.wintereenmasDelve2012.model.game.turnStepAction
{
	public class AttackAction : TurnStepAction
	{
		private Avatar _attacker;
		private Avatar _defender;

		public AttackAction(Avatar attacker, Avatar defender)
			: base(false)
		{
			_attacker = attacker;
			_defender = defender;

			RequiresMovement = false;
			RequiresAction = true;
			HasMoreTurns = false;
			AcceptsAvatarFocus = true;
		}

		override public void Commit(AbstractQuest quest, StoryTeller storyTeller, ChanceProvider chanceProvider)
		{
			int attackerAttackRoll =_attacker.RollAttack(chanceProvider);
			int defenderDefendRoll = _defender.RollDefense(chanceProvider);
			
			int defenderDamageTaken = Math.Max(0, attackerAttackRoll - defenderDefendRoll);

			_defender.TakeBodyDamage(defenderDamageTaken);

			Story story = new Story();
			if (defenderDamageTaken <= 0)
			{ story.Add(storyTeller.NarratorVoice, _attacker.ClassDescription+" misses "+_defender.ClassDescription+"."); }
			else if (!_defender.IsAlive)
			{
				quest.RemoveAvatar(_defender);
				story.Add(storyTeller.NarratorVoice, _attacker.ClassDescription + " slays " + _defender.ClassDescription + " with reckless abandon!");
			}
			else
			{ story.Add(storyTeller.NarratorVoice, _attacker.ClassDescription + " damages " + _defender.ClassDescription + " for " + defenderDamageTaken + " damage!"); }

			storyTeller.StoryComplete += OnStoryComplete;
			storyTeller.TellStory(story);
		}

		private void OnStoryComplete(object sender, EventArgs args)
		{ base.DoComplete(); }
	}
}
