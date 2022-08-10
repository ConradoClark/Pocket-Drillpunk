﻿using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class BattleIntro : BaseUIObject
    {
        public ScriptPrefab EnemyIntro;
        public MeshRenderer GameRenderer;
        public Material WhiteMaterial;
        public Color BattleColor;

        public Vector3 EnemyPosition;

        public Transform TopFrame;
        public Vector3 TopFrameTargetPosition;
        public Transform BattleFrame;
        public Vector3 BattleFrameInitialPosition;
        public Vector3 BattleFrameTargetPosition;

        private Vector3 _originalTopFramePos;
        private Player _player;
        private Material _originalGameRendererMaterial;
        private DrillBattler _drillBattler;
        private GlobalSceneLight _sceneLight;
        private Color _originalSceneLightColor;

        private BattleSequence _battleSequence;

        protected override void OnAwake()
        {
            base.OnAwake();
            _originalTopFramePos = TopFrame.position;
            _player = SceneObject<Player>.Instance(true);
            _originalGameRendererMaterial = GameRenderer.material;
            _drillBattler = SceneObject<DrillBattler>.Instance(true);
            _sceneLight = SceneObject<GlobalSceneLight>.Instance();
            _originalSceneLightColor = _sceneLight.Light.color;
            _battleSequence = SceneObject<BattleSequence>.Instance();
            BattleFrame.transform.position = BattleFrameInitialPosition;
        }

        public void EnterBattle(ScriptPrefab enemyBattler)
        {
            _player.gameObject.SetActive(false); // disables the player
            DefaultMachinery.AddBasicMachine(_drillBattler.ActivateAndMoveToPosition());
            DefaultMachinery.AddBasicMachine(ShowBattleIntro(enemyBattler));
        }

        private IEnumerable<IEnumerable<Action>> ShowBattleIntro(ScriptPrefab enemyBattler)
        {
            // spawn intro
            EnemyIntro.Pool.TryGetFromPool(out _);
            // spawn spinning player, enemy outline, then enemy

            yield return HideTopFrame().AsCoroutine();

            _sceneLight.Light.color = BattleColor;
            //yield return TimeYields.WaitSeconds(UITimer, 1f); // wait intro
            yield return ShowBattleFrame().AsCoroutine();
            GameRenderer.material = WhiteMaterial;

            if (enemyBattler.Pool.TryGetFromPool(out var enemy))
            {
                enemy.Component.transform.position = EnemyPosition;
            }

            yield return TimeYields.WaitSeconds(UITimer, 2f);
            _battleSequence.StartBattle();
        }

        private IEnumerable<IEnumerable<Action>> ShowBattleFrame()
        {
            BattleFrame.gameObject.SetActive(true);
            yield return BattleFrame.GetAccessor()
                .Position
                .ToPosition(BattleFrameTargetPosition)
                .Over(1f)
                .UsingTimer(UITimer)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .Build();
        }

        private IEnumerable<IEnumerable<Action>> HideTopFrame()
        {
            yield return TopFrame.GetAccessor()
                .Position
                .ToPosition(transform.position - (transform.position - TopFrameTargetPosition)*0.5f)
                .Over(0.5f)
                .UsingTimer(UITimer)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .Build();

            yield return TopFrame.GetAccessor()
                .Position
                .ToPosition(TopFrameTargetPosition)
                .Over(1.5f)
                .UsingTimer(UITimer)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .Build();
        }
    }
}
