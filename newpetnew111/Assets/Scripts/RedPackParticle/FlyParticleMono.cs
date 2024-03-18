using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace PVM
{
    public class FlyParticleMono : MonoBehaviour
    {
        private Animator mAni;
        private float mOffsetY;
        private const float mInterval = 20f;
        private EFlyParticleType mType;
        private Tween mFlyTween, mDropTween;
        public AnimationCurve mCurve;

        public void InitData(int _idx, Vector3 _initPos, Transform _root, float _beginX, float _offsetY, EFlyParticleType _type)
        {
            mOffsetY = _offsetY;
            mType = _type;
            mAni = GetComponent<Animator>();
            transform.SetParent(_root);
            var _InitX = _beginX + mInterval * _idx;
            transform.localPosition = new Vector3(_InitX, _initPos.y, 0);
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;

            var _targetX = _InitX + Random.Range(-10f, 10f);
            var _targetY = transform.localPosition.y - mOffsetY;
            var _targetPos = new Vector3(_targetX, _targetY, 0);
            
            DoDrop(_targetPos);
        }
        private void DoDrop(Vector3 _targetPos)
        {
            mDropTween = DOTween.Sequence().Insert(0, transform.DOLocalMoveX(_targetPos.x, 0.5f).SetEase(Ease.Linear))
                                           .Insert(0, transform.DOLocalMoveY(_targetPos.y, 0.5f).SetEase(mCurve));
            mDropTween.onComplete = () => {  mAni.Play("Shake");};
        }
        public void MoveToPos(Vector3 vector3, float time)
        {
            mFlyTween = transform.DOMove(vector3, time).OnComplete(() =>
            {
                Dispose();
                FlyParticleEffectManager.Instance.AddEntityToCache(mType, gameObject);
            });
        }
        private void Dispose()
        {
            mDropTween?.Kill();
            mFlyTween.Kill();
        }
    }
}