using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace ScenarioAmender
{
    // Token: 0x02000003 RID: 3
    [StaticConstructorOnStartup]
    public static class Start
    {
        // Token: 0x06000006 RID: 6 RVA: 0x00002270 File Offset: 0x00000470
        static Start()
        {
            void Value()
            {
                Find.WindowStack.Add(new Page_MidGameScenarioEditor());
            }

            typeof(MainMenuDrawer).GetNestedType("<>c", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetField("<>9__16_2", BindingFlags.Static | BindingFlags.Public)
                ?.SetValue(null, (Action) Value);
        }
    }
}