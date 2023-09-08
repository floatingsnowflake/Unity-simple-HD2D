using System;
using UnityEngine;

namespace HD2DProject2.Scripts
{
  public class FollowPlayerRotation : MonoBehaviour
  {
    private Transform _player;

    private void Start()
    {
      _player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
      transform.rotation = _player.rotation;
    }
  }
}