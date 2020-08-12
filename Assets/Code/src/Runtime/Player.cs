﻿using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace HouraiTeahouse.FantasyCrescendo.Authoring {

public class Player : MonoBehaviour, IConvertGameObjectToEntity {

#pragma warning disable 0649
  [SerializeField] CharacterFrameData _frameData;
#pragma warning restore 0649
  
  public void Convert(Entity entity, EntityManager entityManager, 
                      GameObjectConversionSystem conversionSystem) {
    entityManager.AddComponent(entity, typeof(PlayerConfig));
    entityManager.AddComponent(entity, typeof(PlayerInputComponent));
    entityManager.AddComponent(entity, typeof(CharacterFrame));
    entityManager.AddComponentData(entity, new PlayerComponent {
      StateController = _frameData != null ? 
        _frameData.BuildController() : 
        default(BlobAssetReference<CharacterStateController>)
    });

    // Allocate 64 player hitboxes for immediate player use.
    var hitboxArray = CreatePlayerHitboxes(entityManager, CharacterFrame.kMaxPlayerHitboxCount);
    entityManager.AddBuffer<PlayerHitboxBuffer>(entity).AddRange(hitboxArray);
  }


  NativeArray<PlayerHitboxBuffer> CreatePlayerHitboxes(EntityManager entityManager, int size) {
    var hitboxArchetype = entityManager.CreateArchetype(
      typeof(Translation), typeof(Hitbox), typeof(Disabled));
    var hitboxArray = new NativeArray<PlayerHitboxBuffer>(size, Allocator.Temp);
    for (var i = 0; i < size; i++) {
      var entity = entityManager.CreateEntity(hitboxArchetype);
#if UNITY_EDITOR
      entityManager.SetName(entity, $"{name}, Hitbox {i + 1}");
#endif
      hitboxArray[i] = new PlayerHitboxBuffer { Hitbox = entity };
    }
    return hitboxArray;
  }

}

}