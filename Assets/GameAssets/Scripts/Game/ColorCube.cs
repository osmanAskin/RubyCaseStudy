using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using RubyCase.Level;
using RubyCase.Pools;

namespace RubyCase.Game
{
    public class ColorCube : MonoBehaviour, INodeObject, IPoolObject
    {
        [SerializeField] private Renderer cubeRenderer;
        [SerializeField] private Collider cubeCollider;
        public int colorID;

        public ColorCubeNode CurrentNode { get; set; }

        private static MaterialPropertyBlock _mpb;

        public void Initialize(Node node)
        {
            CurrentNode = node as ColorCubeNode;
        }

        public void Init(Color pixelColor, List<LevelData.LevelColorData> levelColors, float threshold)
        {
            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            cubeRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor("_BaseColor", pixelColor.linear);
            cubeRenderer.SetPropertyBlock(_mpb);

            colorID = FindClosestColorID(pixelColor, levelColors, threshold);
        }

        private int FindClosestColorID(Color pixelColor, List<LevelData.LevelColorData> levelColors, float threshold)
        {
            float minDistance = float.MaxValue;
            int bestID = -1;

            foreach (var lc in levelColors)
            {
                float dr = pixelColor.r - lc.color.r;
                float dg = pixelColor.g - lc.color.g;
                float db = pixelColor.b - lc.color.b;
                float distance = Mathf.Sqrt(dr * dr + dg * dg + db * db);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestID = lc.id;
                }
            }

            return bestID;
        }

        public void GetAbsorbed(Transform boxCenter, float duration, Ease moveEase, Ease scaleEase, CollectableBoxManager collectableBoxManager)
        {
            CurrentNode.SetEmpty(this);
            cubeCollider.enabled = false;

            Vector3 startPos = transform.position;
            float t = 0f;

            DOTween.Sequence()
                .Join(DOTween.To(() => t, x =>
                {
                    t = x;
                    transform.position = Vector3.Lerp(startPos, boxCenter.position, t);
                }, 1f, duration).SetEase(moveEase))
                .Join(transform.DOScale(Vector3.zero, duration).SetEase(scaleEase))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    collectableBoxManager.ColorCubeBlasted();
                }).SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        public void Reset()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            cubeCollider.enabled = true;
            colorID = -1;
            CurrentNode = null;
        }
    }
}
