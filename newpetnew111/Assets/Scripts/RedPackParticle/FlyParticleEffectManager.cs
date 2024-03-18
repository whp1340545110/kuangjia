using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PFramework;
using Singleton;

namespace PVM
{
    public enum EFlyParticleType
    {
        eRedEnvelope,
        eEnergy,
    }
    public class FlyParticleEffectManager : MonoSingleton<FlyParticleEffectManager>
    {
        private const int MAX_CACHE_NUM = 50;
        private List<GameObject> mRedEnvelopeParticleCache = new List<GameObject>();
        private List<GameObject> mEnergyParticleCache = new List<GameObject>();
        private bool isLoadComplete = false;
        private GameObject mRedEnvPrefab;
        private GameObject mEnergyPrefab;

        public async void CreateParticle(Vector3 _startWorldPos, Vector3 _endWorldPos, Transform _startRoot, Transform _endRoot, int _count, float _offsetY = 100, EFlyParticleType _type = EFlyParticleType.eRedEnvelope)
        {
#if WHITE_PACK 
            return;
#endif
            if (!isLoadComplete)
            {
                mRedEnvPrefab = await Peach.LoadAsync<GameObject>("FlyParticle/RedPackGO");
                mEnergyPrefab = await Peach.LoadAsync<GameObject>("FlyParticle/EnergyGO");
                isLoadComplete = true;
            }
            AudioMgr.Instance.PlaySound(6);
            StartCoroutine(CreateParticles(_startRoot, _startWorldPos, _endRoot, _endWorldPos, _count, _offsetY, _type));
        }
        
        private IEnumerator CreateParticles(Transform _startRoot, Vector3 _beginWorldPos, Transform _endRoot, Vector3 _endWorldPos, int _num, float _offsetY, EFlyParticleType _type)
        {
            var _envList = new List<GameObject>(_num);
            var _locPos = _startRoot.worldToLocalMatrix.MultiplyPoint3x4(_beginWorldPos);
            var _halfWidth = 20 * _num / 2;
            for (int i = 0; i < _num; i++)
            {
                GameObject _env = GetEntityFromCache(_type);
                var __mono = _env.GetComponent<FlyParticleMono>();  
                __mono?.InitData(i, _locPos, _startRoot, _locPos.x - _halfWidth, _offsetY, _type);
                _envList.Add(_env);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(1f);
            StartCoroutine(DoFly(_envList, _endRoot, _endWorldPos));
        }

        IEnumerator DoFly(List<GameObject> _flyLst, Transform _endRoot, Vector3 _endWorldPos)
        {
            for (int i = 0; i < _flyLst.Count; i++)
            {
               
                _flyLst[i].transform.SetParent(_endRoot);
                _flyLst[i].GetComponent<FlyParticleMono>().MoveToPos(_endWorldPos, 0.3f);
                yield return new WaitForSeconds(0.03f);
            }
        }

        #region Cache
        private GameObject GetEntityFromCache(EFlyParticleType _type)
        {
            GameObject _entity = null;
            List<GameObject> _cache = _type == EFlyParticleType.eRedEnvelope ? mRedEnvelopeParticleCache : mEnergyParticleCache;
            if (_cache.Count > 0)
            {
                _entity = _cache[0];
                _cache.RemoveAt(0);
            }
            else
            {
                var _prefab = _type == EFlyParticleType.eRedEnvelope ? mRedEnvPrefab : mEnergyPrefab;
                _entity = GameObject.Instantiate(_prefab);
                ResetEntity(_entity);
            }
            _entity.gameObject.SetActive(true);
            return _entity;
        }
        public void AddEntityToCache(EFlyParticleType _type, GameObject _obj)
        {
            List<GameObject> _cache = _type == EFlyParticleType.eRedEnvelope ? mRedEnvelopeParticleCache : mEnergyParticleCache;
            if (_cache.Count < MAX_CACHE_NUM)
            {
                ResetEntity(_obj);
                _cache.Add(_obj);
            }
            else
                GameObject.Destroy(_obj);
        }
        private void ResetEntity(GameObject _obj)
        {
            _obj.gameObject.SetActive(false);
            _obj.transform.SetParent(transform);
            _obj.transform.localPosition = Vector3.zero;
            _obj.transform.localScale = Vector3.one;
            _obj.transform.localRotation = Quaternion.identity;
        }
        #endregion
    }
}