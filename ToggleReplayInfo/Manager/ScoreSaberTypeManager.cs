using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using ToggleReplayInfo.Exceptions;
using ToggleReplayInfo.Models;
using ToggleReplayInfo.TypeReflection;
using UnityEngine;
using Zenject;
using static ToggleReplayInfo.TypeReflection.TypeDefinitionManager;
using static ToggleReplayInfo.TypeReflection.Utilities;
using static ToggleReplayInfo.TypeReflection.TypeDefinitionManager.TypeDefinition.DefinitionInfo;
using UnityEngine.UI;

namespace ToggleReplayInfo.Manager
{
    public class ScoreSaberTypeManager : IInitializable
    {
        private readonly TypeDefinitionManager _typeDefinitionManager;

        private bool _isReady = false;

        public Assembly ScoreSaberAssembly { get; private set; }
        //public Type LegacyReplayPlayer { get; internal set; }
        //public Type NewReplayPlayer { get; internal set; }
        public Type ReplayMetaData { get; internal set; }
        public Type ReplayCoreData { get; internal set; }
        public Type ScoreSaberReplay { get; internal set; }
        public Type ScoreSaberReplayContainer { get; internal set; }
        public Type BigStaticThing { get; internal set; }
        public Type LeaderboardPageScoreContainer { get; internal set; }
        public Type ScoreSaberLevelResultsViewController { get; internal set; }
        public Type LeaderboardInfoDownloader { get; internal set; }

        public ScoreSaberTypeManager(TypeDefinitionManager scoreSaberTypeDefinitionManager)
        {
            _typeDefinitionManager = scoreSaberTypeDefinitionManager;
        }

        public bool IsReady
        {
            get => _isReady;
        }

        public void Initialize()
        {
            ScoreSaberAssembly = Assembly.GetAssembly(typeof(ScoreSaber.Plugin));

            RegisterTypeDefinitions();

            //LegacyReplayPlayer = ScoreSaberAssembly.GetType("ScoreSaber.LegacyReplayPlayer");

            _typeDefinitionManager.SetLoggingAction(Logger.log.Debug);

            try
            {
                int c = 0;
                while(!_typeDefinitionManager.ResolveAllTypes())
                {
                    c++;
                    if (c > 100) throw new ReplayTextInitializationException("Too many iterations for type resolution, aborting!");
                }

                PostInitialize();
            }
            catch(Exception ex)
            {
                Logger.log.Error($"{nameof(ScoreSaberTypeManager)} failed: {ex.Message}");
                Logger.log.Error($"{ex.StackTrace}");
            }
        }

        private void RegisterTypeDefinitions()
        {
            /*var NewReplayPlayerDef = new TypeDefinition(definitionName: nameof(NewReplayPlayer))
                .AddFieldDefinition(typeof(int), MemberVisibility.Private)
                .AddFieldDefinition(typeof(MainCamera), MemberVisibility.Private, isReadonly: true)
                .AddFieldDefinition(typeof(SaberManager), MemberVisibility.Private, isReadonly: true)
                // KeyframeArray
                .AddFieldDefinition(typeof(MainSettingsModelSO), MemberVisibility.Private, isReadonly: true)
                .AddFieldDefinition(typeof(IReturnToMenuController), MemberVisibility.Private, isReadonly: true)
                .AddFieldDefinition(typeof(Camera), MemberVisibility.Private)
                .AddFieldDefinition(typeof(Camera), MemberVisibility.Private)
                .AddFieldDefinition(typeof(bool), MemberVisibility.Private)
                .AddFieldDefinition(typeof(Vector3), MemberVisibility.Private)
                .BindToProperty(this, nameof(NewReplayPlayer));*/

            var ReplayMetaDataDef = new TypeDefinition(definitionName: nameof(ReplayMetaData))
                .AddMultipleSameFieldDefinitions(5, typeof(string), MemberVisibility.Private) // playerId, name, mods, timeset, country
                .AddMultipleSameFieldDefinitions(7, typeof(int), MemberVisibility.Private) // rank, score, badCuts, missedNotes, maxCombo, fullCombo, hmd
                .AddMultipleSameFieldDefinitions(2, typeof(double), MemberVisibility.Private) // pp, weight
                .AddFieldDefinition(typeof(string), MemberVisibility.Public)
                .AddFieldDefinition(typeof(bool), MemberVisibility.Public)
                .AddFieldDefinition(typeof(double), MemberVisibility.Public)
                .AddFieldDefinition(typeof(GameplayModifiers), MemberVisibility.Public)
                .BindToProperty(this, nameof(ReplayMetaData));

            var ReplayCoreDataDef = new TypeDefinition(definitionName: nameof(ReplayCoreData))
                .AddMultipleSameFieldDefinitions(4, typeof(string), MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(int), MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(string[]), MemberVisibility.Assembly)
                .AddMultipleSameFieldDefinitions(4, typeof(float), MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(bool), MemberVisibility.Assembly)
                .BindToProperty(this, nameof(ReplayCoreData));

            var ScoreSaberReplayDef = new TypeDefinition(definitionName: nameof(ScoreSaberReplay))
                .AddFieldDefinition(ReplayCoreDataDef, MemberVisibility.Assembly)
                .AddMultipleSameFieldDefinitions(7, typeof(IList), MemberVisibility.Assembly)
                .BindToProperty(this, nameof(ScoreSaberReplay));

            var ScoreSaberReplayContainerDef = new TypeDefinition(definitionName: nameof(ScoreSaberReplayContainer))
                .AddMultipleSameFieldDefinitions(2, typeof(string), MemberVisibility.Public)
                .AddMultipleSameFieldDefinitions(2, typeof(bool), MemberVisibility.Public)
                .AddFieldDefinition(typeof(byte[]), MemberVisibility.Public)
                .AddFieldDefinition(ScoreSaberReplayDef, MemberVisibility.Public)
                .AddFieldDefinition(typeof(IDifficultyBeatmap), MemberVisibility.Public)
                .AddFieldDefinition(typeof(GameplayModifiers), MemberVisibility.Public)
                .BindToProperty(this, nameof(ScoreSaberReplayContainer));


            var BigStaticThingDef = new TypeDefinition(MemberVisibility.Assembly, isStatic: true, definitionName: nameof(BigStaticThing))
                .AddMultipleSameFieldDefinitions(3, typeof(bool), MemberVisibility.Private, isStatic: true)
                .AddFieldDefinition(ScoreSaberReplayContainerDef, MemberVisibility.Private, isStatic: true)
                .AddMethodDefinition(ScoreSaberReplayContainerDef, MemberVisibility.Assembly, isStatic: true)
                .BindToProperty(this, nameof(BigStaticThing));

            var LeaderboardPageScoreContainerDef = new TypeDefinition(MemberVisibility.Assembly, isSealed: true, definitionName: nameof(LeaderboardPageScoreContainer))
                .AddPropertyDefinition(typeof(string), MemberVisibility.Assembly) // ranked
                .AddPropertyDefinition(typeof(string), MemberVisibility.Assembly) // uid
                .AddPropertyDefinition(typeof(IList), MemberVisibility.Assembly) // scores (List<ReplayMetaData>)
                .AddPropertyDefinition(typeof(string), MemberVisibility.Assembly) // playerScore
                .AddFieldDefinition(typeof(IDifficultyBeatmap), MemberVisibility.Assembly) // level
                .AddFieldDefinition(typeof(string), MemberVisibility.Private)
                .AddFieldDefinition(typeof(string), MemberVisibility.Private)
                .AddFieldDefinition(typeof(IList), MemberVisibility.Private)
                .AddFieldDefinition(typeof(string), MemberVisibility.Private)
                .BindToProperty(this, nameof(LeaderboardPageScoreContainer));

            var ScoreSaberLevelResultsViewControllerDef = new TypeDefinition(MemberVisibility.Assembly, isSealed: true, definitionName: nameof(ScoreSaberLevelResultsViewController))
                .AddFieldDefinition(typeof(ResultsViewController), MemberVisibility.Private)
                .AddFieldDefinition(typeof(IDifficultyBeatmap), MemberVisibility.Private)
                .AddFieldDefinition(typeof(LevelCompletionResults), MemberVisibility.Private)
                .AddFieldDefinition(typeof(Button), MemberVisibility.Family, isReadonly: true)
                .BindToProperty(this, nameof(ScoreSaberLevelResultsViewController));

            var LeaderboardInfoDownloaderDef = new TypeDefinition(MemberVisibility.Assembly, isSealed: true, definitionName: nameof(LeaderboardInfoDownloader))
                .AddFieldDefinition(typeof(bool), MemberVisibility.Private)
                .AddFieldDefinition(typeof(bool), MemberVisibility.Private)
                .AddFieldDefinition(LeaderboardPageScoreContainerDef, MemberVisibility.Private)
                .AddFieldDefinition(typeof(int), MemberVisibility.Private)
                .AddFieldDefinition(typeof(string), MemberVisibility.Private)
                .AddFieldDefinition(typeof(string), MemberVisibility.Private)
                // ScoreSaberLeaderboardView
                .AddFieldDefinition(typeof(bool), MemberVisibility.Private)
                .BindToProperty(this, nameof(LeaderboardInfoDownloader));

            _typeDefinitionManager.RegisterTypes(ScoreSaberAssembly, new TypeDefinition[] {
                ReplayMetaDataDef,
                ReplayCoreDataDef,
                ScoreSaberReplayDef,
                ScoreSaberReplayContainerDef,
                BigStaticThingDef,
                LeaderboardPageScoreContainerDef,
                ScoreSaberLevelResultsViewControllerDef,
                LeaderboardInfoDownloaderDef
            });
        }

        private void PostInitialize()
        {
            foreach(PropertyInfo propertyInfo in typeof(ScoreSaberTypeManager).GetProperties(TypeReflection.Utilities.AnyBindingFlags))
            {
                if(propertyInfo.GetValue(this) == null)
                {
                    throw new ReplayTextInitializationException($"Couldn't find {propertyInfo.PropertyType} \"{propertyInfo.Name}\" from ScoreSaber!");
                }
            }

            if (!ReplayMetaDataWrapper.HasBeenSetup())
            {
                ReplayMetaDataWrapper.InitialSetup(ReplayMetaData, LeaderboardPageScoreContainer);
                LeaderboardPageScoresContainerWrapper.InitialSetup(LeaderboardPageScoreContainer, LeaderboardInfoDownloader);
            }

            if (!AllMembersPopulated(MemberTypes.Property, this))
            {
                throw new ReplayTextInitializationException($"Not all members in {nameof(ScoreSaberTypeManager)} were populated!");
            }

            _isReady = true;
        }
    }
}
