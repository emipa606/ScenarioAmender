using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace ScenarioAmender
{
    // Token: 0x02000002 RID: 2
    public class Page_MidGameScenarioEditor : Page_ScenarioEditor
    {
        // Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
        public Page_MidGameScenarioEditor() : base(Find.Scenario.CopyForEditing())
        {
            var method = typeof(Page_ScenarioEditor).GetMethod("CheckAllPartsCompatible",
                BindingFlags.Static | BindingFlags.NonPublic);
            CheckAllPartsCompatible =
                Delegate.CreateDelegate(typeof(Predicate<Scenario>), method!) as Predicate<Scenario>;
            nextAct = SaveAndClose;
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        private Predicate<Scenario> CheckAllPartsCompatible { get; }

        // Token: 0x06000003 RID: 3 RVA: 0x000020B9 File Offset: 0x000002B9
        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);
            DoBottomButtons(rect, "Save".Translate());
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020DC File Offset: 0x000002DC
        public void SaveAndClose()
        {
            var list = new List<GameCondition>();
            Find.CurrentMap.GameConditionManager.GetAllGameConditionsAffectingMap(Find.CurrentMap, list);
            foreach (var gameCondition in list)
            {
                gameCondition.Permanent = false;
            }

            (typeof(GameRules).GetField("disallowedDesignatorTypes", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(Current.Game.Rules) as HashSet<Type>)?.Clear();
            (typeof(GameRules).GetField("disallowedBuildings", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(Current.Game.Rules) as HashSet<ThingDef>)?.Clear();
            Current.Game.Scenario = EditingScenario;
            foreach (var scenPart in Current.Game.Scenario.AllParts)
            {
                if (scenPart is ScenPart_PlayerFaction)
                {
                    typeof(ScenPart_PlayerFaction)
                        .GetField("factionDef", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.SetValue(scenPart, Find.FactionManager.OfPlayer.def);
                }
                else if ((!(scenPart is ScenPart_GameStartDialog) || Find.GameInitData != null) &&
                         !(scenPart is ScenPart_ConfigPage_ConfigureStartingPawns))
                {
                    scenPart.PostGameStart();
                    scenPart.PostWorldGenerate();
                    scenPart.GenerateIntoMap(Find.CurrentMap);
                }
            }

            Close();
        }

        // Token: 0x06000005 RID: 5 RVA: 0x0000225C File Offset: 0x0000045C
        protected override bool CanDoNext()
        {
            return CheckAllPartsCompatible(EditingScenario);
        }
    }
}