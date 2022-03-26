using System;
using System.Collections;
using System.Reflection;
using ToggleReplayInfo.Exceptions;
using ToggleReplayInfo.TypeReflection;
using ToggleReplayInfo.TypeReflection.Attributes;
using ToggleReplayInfo.TypeReflection.Core;
using UnityEngine.UI;
using Zenject;
using static ToggleReplayInfo.TypeReflection.Utilities;

namespace ToggleReplayInfo.Manager
{
    public class ScoreSaberTypeManager : IInitializable
    {
        private readonly TypeDefinitionManager _typeDefinitionManager;

        private bool _isReady = false;

        public static bool HasErrorsOnInitStatic { get; private set; } = false;
        public bool HasErrorsOnInit => HasErrorsOnInitStatic;

        public ScoreSaberTypes SSTypes = new ScoreSaberTypes();

        public Assembly ScoreSaberAssembly { get; private set; }

        public class ScoreSaberTypes
        {
            public Type SS_ReplayMetadataContainer { get; internal set; }
            public Type ReplayMetaData { get; internal set; }
            public Type LeaderboardPlayerInfo { get; internal set; }
            public Type ScoreSaberLevelResultsViewController { get; internal set; }

            /*[IgnoreVerification] public Type ReplayCoreData { get; internal set; }
            [IgnoreVerification] public Type ScoreSaberReplay { get; internal set; }
            [IgnoreVerification] public Type ScoreSaberReplayContainer { get; internal set; }
            [IgnoreVerification] public Type ScoreSaberLeaderboardScoreEntry { get; internal set; }
            [IgnoreVerification] public Type SS_DifficultyLeaderboardData { get; internal set; }*/
        }

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

            _typeDefinitionManager.SetLoggingAction(Logger.Log.Debug);

            try
            {
                int c = 0;
                while(!_typeDefinitionManager.ResolveAllTypes())
                {
                    c++;
                    if (c > 20) throw new ReplayTextInitializationException("Too many iterations for type resolution, aborting!");
                }

                PostInitialize();

                HasErrorsOnInitStatic = false;
            }
            catch(Exception ex)
            {
                Logger.Log.Error($"{nameof(ScoreSaberTypeManager)} failed: {ex.Message}");
                Logger.Log.Error($"{ex.StackTrace}");
                Logger.Log.Error($"Unresolved Types: {_typeDefinitionManager.GetUnresolvedTypes(ScoreSaberAssembly)}");
                HasErrorsOnInitStatic = true;
            }

            Plugin.SSTM = this;
        }

        private void RegisterTypeDefinitions()
        {
            // Important
            var LeaderboardPlayerInfoDef = new TypeDefinition(isSealed: true, definitionName: nameof(ScoreSaberTypes.LeaderboardPlayerInfo))
                .AddMultipleSameFieldDefinitions(5, typeof(string), MemberVisibility.Private) // id, name, profilePicture, country, role
                .AddFieldDefinition(typeof(int), MemberVisibility.Private) // permissions
                .AddPropertyWrappersForAllCurrentlyAddedFields(MemberVisibility.Assembly)
                .WithLimits(new TypeDefinition.MemberLimits { MaxProperties = 6 })
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.LeaderboardPlayerInfo))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);

            // Important
            var ReplayMetaDataDef = new TypeDefinition(isSealed: true, definitionName: nameof(ScoreSaberTypes.ReplayMetaData))
                .AddMultipleSamePropertyDefinitions(8, typeof(int), MemberVisibility.Assembly) // id, rank, baseScore, modifiedScore, badCuts, missedNotes, maxCombo, hmd
                .AddMultipleSamePropertyDefinitions(3, typeof(double), MemberVisibility.Assembly) // pp, weight, multiplier
                .AddMultipleSamePropertyDefinitions(2, typeof(bool), MemberVisibility.Assembly) // fullCombo, hasReplay
                .AddPropertyDefinition(typeof(string), MemberVisibility.Assembly) // modifiers
                .AddPropertyDefinition(typeof(DateTime), MemberVisibility.Assembly) // timeSet
                .AddPropertyDefinition(LeaderboardPlayerInfoDef, MemberVisibility.Assembly)
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.ReplayMetaData))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);

            // Important
            var SS_ReplayMetadataContainerDef = new TypeDefinition(definitionName: nameof(ScoreSaberTypes.SS_ReplayMetadataContainer))
                .AddFieldDefinition(ReplayMetaDataDef, MemberVisibility.Private)
                .AddFieldDefinition(typeof(bool), MemberVisibility.Private)
                // Something With Song Info
                .AddFieldDefinition(typeof(double), MemberVisibility.Private)
                .AddFieldDefinition(typeof(GameplayModifiers), MemberVisibility.Private)
                .AddFieldDefinition(typeof(string), MemberVisibility.Private)
                .WithLimits(new TypeDefinition.MemberLimits { MaxFields = 6 })
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.SS_ReplayMetadataContainer))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);

            /*var ScoreSaberLeaderboardScoreEntryDef = new TypeDefinition(isSealed: true, definitionName: nameof(ScoreSaberTypes.ScoreSaberLeaderboardScoreEntry))
                .AddFieldDefinition(ReplayMetaDataDef, MemberVisibility.Private)
                .AddFieldDefinition(typeof(bool), MemberVisibility.Private)
                .AddFieldDefinition(typeof(double), MemberVisibility.Private)
                .AddFieldDefinition(typeof(GameplayModifiers), MemberVisibility.Private)
                .AddFieldDefinition(typeof(string), MemberVisibility.Private)
                .WithLimits(new TypeDefinition.MemberLimits { MaxFields = 6})
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.ScoreSaberLeaderboardScoreEntry))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);

            *//*var ScoreSaberSongInfoDef = new TypeDefinition(definitionName: nameof(ScoreSaberSongInfo))
                .BindToProperty(this, nameof(ScoreSaberSongInfo))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);*//*

            var ReplayCoreDataDef = new TypeDefinition(definitionName: nameof(ScoreSaberTypes.ReplayCoreData))
                .AddMultipleSameFieldDefinitions(4, typeof(string), MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(int), MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(string[]), MemberVisibility.Assembly)
                .AddMultipleSameFieldDefinitions(4, typeof(float), MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(bool), MemberVisibility.Assembly)
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.ReplayCoreData))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);


            var SS_DifficultyLeaderboardDataDef = new TypeDefinition(definitionName: nameof(ScoreSaberTypes.SS_DifficultyLeaderboardData))
                .AddMultipleSameFieldDefinitions(2, typeof(int), MemberVisibility.Private)
                .AddMultipleSameFieldDefinitions(2, typeof(string), MemberVisibility.Private)
                .AddPropertyWrappersForAllCurrentlyAddedFields(MemberVisibility.Assembly)
                .WithLimits(new TypeDefinition.MemberLimits { MaxFields = 4, MaxProperties = 4 })
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.SS_DifficultyLeaderboardData))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);

            var ScoreSaberReplayDef = new TypeDefinition(definitionName: nameof(ScoreSaberTypes.ScoreSaberReplay))
                .AddFieldDefinition(ReplayCoreDataDef, MemberVisibility.Assembly)
                .AddMultipleSameFieldDefinitions(7, typeof(IList), MemberVisibility.Assembly)
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.ScoreSaberReplay))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);

            var ScoreSaberReplayContainerDef = new TypeDefinition(definitionName: nameof(ScoreSaberTypes.ScoreSaberReplayContainer))
                .AddMultipleSameFieldDefinitions(2, typeof(string), MemberVisibility.Assembly)
                .AddMultipleSameFieldDefinitions(2, typeof(bool), MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(byte[]), MemberVisibility.Assembly)
                .AddFieldDefinition(ScoreSaberReplayDef, MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(IDifficultyBeatmap), MemberVisibility.Assembly)
                .AddFieldDefinition(typeof(GameplayModifiers), MemberVisibility.Assembly)
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.ScoreSaberReplayContainer))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);*/

            var ScoreSaberLevelResultsViewControllerDef = new TypeDefinition(MemberVisibility.Assembly, isSealed: true, definitionName: nameof(ScoreSaberTypes.ScoreSaberLevelResultsViewController))
                .AddFieldDefinition(typeof(ResultsViewController), MemberVisibility.Private)
                .AddFieldDefinition(typeof(IDifficultyBeatmap), MemberVisibility.Private)
                .AddFieldDefinition(typeof(LevelCompletionResults), MemberVisibility.Private)
                .AddFieldDefinition(typeof(Button), MemberVisibility.Family, isReadonly: true)
                .BindToProperty(SSTypes, nameof(ScoreSaberTypes.ScoreSaberLevelResultsViewController))
                .Register(_typeDefinitionManager, ScoreSaberAssembly);

        }

        private void PostInitialize()
        {
            if (!AllMembersPopulated(MemberTypes.Property, SSTypes))
            {
                throw new ReplayTextInitializationException($"Not all members in {nameof(ScoreSaberTypeManager.ScoreSaberTypes)} were populated!");
            }

            _isReady = true;
        }
    }
}
