using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace HD2DProject2.Scripts
{
  public class PlayerVisibleOcclusion : MonoBehaviour
  {
    public enum Type
    {
      ReduceAlpha,
      SwitchMaterial,
    }

    public Type type = Type.ReduceAlpha;

    public bool autoFindRenderer;
    public SpriteRenderer[] spriteRenderers;
    public MeshRenderer[] meshRenderers;

    public bool debug;

    private Transform _player;
    private Transform _camera;
    private bool _processed;
    private Dictionary<MeshRenderer, Material[]> _meshMaterialDict = new Dictionary<MeshRenderer, Material[]>();

    private void Start()
    {
      _camera = Camera.main.transform;
      _player = GameObject.FindWithTag("Player").transform;

      if (autoFindRenderer)
      {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
          _meshMaterialDict[meshRenderer] = meshRenderer.materials;
        }
      }
    }

    private bool IsBeforePlayer()
    {
      var dir1 = transform.position - _player.position;
      var dir2 = _camera.position - _player.position;
      
      var angle = Vector2.Angle(new Vector2(dir1.x, dir1.z), new Vector2(dir2.x, dir2.z));
      return angle < 90f;
    }
    
    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.CompareTag("PlayerVisibleOcclusionCollider") && IsBeforePlayer())
      {
        ProcessOcclusion();
      }
    }

    private void OnTriggerStay(Collider other)
    {
      if (other.gameObject.CompareTag("PlayerVisibleOcclusionCollider"))
      {
        if (IsBeforePlayer())
        {
          ProcessOcclusion();
        }
        else
        {
          ResumeOcclusion();
        }
      }
    }

    private void ProcessOcclusion()
    {
      if (_processed) return;
      _processed = true;
      if (type == Type.ReduceAlpha)
      {
        foreach (var spriteRenderer in spriteRenderers)
        {
          spriteRenderer.DOKill();
          spriteRenderer.DOFade(0.3f, 0.2f);
        }
      }

      if (type == Type.SwitchMaterial)
      {
        foreach (var meshRenderer in meshRenderers)
        {
          meshRenderer.material = _meshMaterialDict[meshRenderer][1];
        }
      }
    }

    private void ResumeOcclusion()
    {
      if (!_processed) return;
      _processed = false;
      foreach (var spriteRenderer in spriteRenderers)
      {
        spriteRenderer.DOKill();
        spriteRenderer.DOFade(1f, 0.2f);
      }
      foreach (var meshRenderer in meshRenderers)
      {
        meshRenderer.material = _meshMaterialDict[meshRenderer][0];
      }
    }

    private void OnTriggerExit(Collider other)
    {
      if (other.gameObject.CompareTag("PlayerVisibleOcclusionCollider"))
      {
        ResumeOcclusion();
      }
    }
  }
}