using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBerry;

public enum eMoveDirection
{
    eMD_Left = 0,
    eMD_Right,
    eMD_Up,
    eMD_Down,

    eMD_LeftUp,
    eMD_LeftDown,
    eMD_RightUP,
    eMD_RightDown,

    eMD_Max,
}

public enum eScale
{
    eScale_X,
    eScale_Y,
    eScale_Z,

    eScale_XY,

    eScale_Max,
}

public enum eSizeDelta
{
    eSizeDelta_X,
    eSizeDelta_Y,

    eSizeDelta_Max,
}

public enum eRotation
{
    eRotation_X,
    eRotation_Y,
    eRotation_Z,

    eRotation_Max,
}

public class DirectionUtil : MonoSingleton<DirectionUtil>
{
    private Dictionary<Transform, Coroutine> _colorcoroutine = new Dictionary<Transform, Coroutine>();
    private Dictionary<Transform, Coroutine> _positioncoroutine = new Dictionary<Transform, Coroutine>();
    private Dictionary<Transform, Coroutine> _scalecoroutine = new Dictionary<Transform, Coroutine>();
    private Dictionary<Transform, Coroutine> _rotationcoroutine = new Dictionary<Transform, Coroutine>();

    private Dictionary<Image, Coroutine> _masksizedeltacoroutine = new Dictionary<Image, Coroutine>();
    private Dictionary<Image, Coroutine> _fillamountcoroutine = new Dictionary<Image, Coroutine>();
    private Dictionary<Image, Coroutine> _imageswitchcoroutine = new Dictionary<Image, Coroutine>();

    List<Transform> errorKey = new List<Transform>(); // 이상한 Key값 찾기용
    List<Image> imageerrorKey = new List<Image>();    

    protected override void Init()
    {
        if (_colorcoroutine == null)
            _colorcoroutine = new Dictionary<Transform, Coroutine>();

        if (_positioncoroutine == null)
            _positioncoroutine = new Dictionary<Transform, Coroutine>();

        if (_scalecoroutine == null)
            _scalecoroutine = new Dictionary<Transform, Coroutine>();

        if (_rotationcoroutine == null)
            _rotationcoroutine = new Dictionary<Transform, Coroutine>();

        if (_masksizedeltacoroutine == null)
            _masksizedeltacoroutine = new Dictionary<Image, Coroutine>();

        if (_fillamountcoroutine == null)
            _fillamountcoroutine = new Dictionary<Image, Coroutine>();

        if (_imageswitchcoroutine == null)
            _imageswitchcoroutine = new Dictionary<Image, Coroutine>();

        if (errorKey == null)
            errorKey = new List<Transform>();

        if (imageerrorKey == null)
            imageerrorKey = new List<Image>();
    }

    private void LateUpdate()
    {
        CleanUpDictionary(_colorcoroutine);
        CleanUpDictionary(_positioncoroutine);
        CleanUpDictionary(_scalecoroutine);
        CleanUpDictionary(_rotationcoroutine);

        CleanUpDictionary_Image(_masksizedeltacoroutine);
        CleanUpDictionary_Image(_fillamountcoroutine);
        CleanUpDictionary_Image(_imageswitchcoroutine);
    }

    private void CleanUpDictionary(Dictionary<Transform, Coroutine> checkDic)
    {
        errorKey.Clear();

        foreach (KeyValuePair<Transform, Coroutine> dic in checkDic)
        {
            if (dic.Key == null)
            {
                if (dic.Value != null)
                    StopCoroutine(dic.Value);

                errorKey.Add(dic.Key);
            }
            else
            {
                if (dic.Value == null)
                    errorKey.Add(dic.Key);
            }
        }

        for (int i = 0; i < errorKey.Count; ++i)
        { // foreach에서 Remove를 하면 에러가나서 for문으로
            checkDic.Remove(errorKey[i]);
        }
    }

    //이미지용
    private void CleanUpDictionary_Image(Dictionary<Image, Coroutine> checkDic)
    {
        imageerrorKey.Clear();

        foreach (KeyValuePair<Image, Coroutine> dic in checkDic)
        {
            if (dic.Key == null)
            {
                if (dic.Value != null)
                    StopCoroutine(dic.Value);

                imageerrorKey.Add(dic.Key);
            }
            else
            {
                if (dic.Value == null)
                    imageerrorKey.Add(dic.Key);
            }
        }

        for (int i = 0; i < imageerrorKey.Count; ++i)
        { // foreach에서 Remove를 하면 에러가나서 for문으로
            checkDic.Remove(imageerrorKey[i]);
        }
    }

    private void RemoveCoroutine(Dictionary<Transform, Coroutine> dic, Transform key)
    {
        if (dic.ContainsKey(key))
        {
            Coroutine coroutine = null;
            dic.TryGetValue(key, out coroutine);
            if (coroutine != null)
                StopCoroutine(coroutine);

            dic.Remove(key);
        }
    }

    private void RemoveimageCoroutine(Dictionary<Image, Coroutine> dic, Image key)
    {
        if (dic.ContainsKey(key))
        {
            Coroutine coroutine = null;
            dic.TryGetValue(key, out coroutine);
            if (coroutine != null)
                StopCoroutine(coroutine);

            dic.Remove(key);
        }
    }

    public void PlayColorDirection(Transform target, float duration, bool isvisible, float delayTime = 0.0f, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_colorcoroutine, target);

        _colorcoroutine.Add(target, StartCoroutine(PlayColor(target, duration, isvisible, delayTime, onFinish)));
    }

    IEnumerator PlayColor(Transform target, float dura, bool isvisible, float delayTime = 0.0f, System.Action onFinish = null)
    {
        List<Graphic> childgrapic = target.GetComponentsInAllChildren<Graphic>();
        List<Color> childcolor = new List<Color>();
        for (int i = 0; i < childgrapic.Count; ++i)
        {
            Color InitColor = childgrapic[i].color;
            InitColor.a = isvisible == true ? 0.0f : 1.0f;
            childgrapic[i].color = InitColor;
            childcolor.Add(childgrapic[i].color);
        }

        yield return new WaitForSeconds(delayTime);

        if (target == null)
            yield break;

        float startTime = Time.time;

        float endTime = Time.time + dura;

        while (endTime >= Time.time)
        {
            if (target == null)
                yield break;

            for (int i = 0; i < childgrapic.Count; ++i)
            {
                float alphaColor = 0.0f;
                if (isvisible)
                    alphaColor = (MathDatas.Sin((90.0f * ((Time.time - startTime) / dura))));
                else
                    alphaColor = 1.0f - (MathDatas.Sin((90.0f * ((Time.time - startTime) / dura))));
                Color dircolor = childcolor[i];
                dircolor.a = alphaColor;
                childgrapic[i].color = dircolor;
            }

            yield return null;
        }

        if (target == null)
            yield break;

        for (int i = 0; i < childgrapic.Count; ++i)
        {
            Color InitColor = childgrapic[i].color;
            InitColor.a = isvisible == false ? 0.0f : 1.0f;
            childgrapic[i].color = InitColor;
        }

        if (onFinish != null)
            onFinish();
    }

    public void PlayPointMoveDirection(Transform target, Vector3 endPos, float _startTime, float _endTime, System.Action onStart = null, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_positioncoroutine, target);

        _positioncoroutine.Add(target, StartCoroutine(PlayPointMoveDirection(target, endPos, _startTime, _endTime, onStart, onFinish)));
    }

    IEnumerator PlayPointMoveDirection(Transform target, Vector3 endPos, float _startTime, float _endTime, System.Action onStart = null, System.Action onFinish = null)
    {
        /*
         * target = 움직일 타겟
         * endPos = 끝날 포지션
         * _startTime = 몇초부터 움직일건가
         * _endTime = 몇초까지 움직일건가
         * onStart = 연출시작 때 부를 함수
         * onFinish = 연출이 끝나고 부를 함수
         */

        yield return new WaitForSeconds(_startTime);

        if (onStart != null)
            onStart();

        if (target == null)
            yield break;

        Vector3 OriginPos = target.transform.localPosition;


        Vector3 PosGap = endPos - OriginPos;

        float duration = _endTime - _startTime;
        float PlayTime = Time.time + duration;
        float StartPlayTime = Time.time;

        while (PlayTime >= Time.time)
        {
            Vector3 pos = OriginPos + (PosGap * (MathDatas.Sin((90.0f * (Time.time - StartPlayTime) / duration))));

            target.localPosition = pos;

            yield return null;
        }

        target.localPosition = endPos;

        if (onFinish != null)
            onFinish();
    }

    public void PlayWaveMoveDirection(Transform target, eMoveDirection _eDrection, float _startTime, float _endTime, float _gradient, bool _overturn, int _wavevolum = 3, System.Action onStart = null, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_positioncoroutine, target);

        _positioncoroutine.Add(target, StartCoroutine(PlayWaveMoveDirection(target, _eDrection, _startTime, _endTime, _gradient, _overturn, _wavevolum, onStart, onFinish)));
    }

    IEnumerator PlayWaveMoveDirection(Transform DirectionTarget, eMoveDirection _eDrection, float _startTime, float _endTime, float _gradient, bool _overturn, int _wavevolum = 3, System.Action onStart = null, System.Action onFinish = null)
    {
        /*
         * DirectionTarget = 움직일 타겟
         * _eDrection = 움직일 방향
         * _startTime = 몇초부터 움직일건가
         * _endTime = 몇초까지 움직일건가
         * _gradient = 어디서부터 혹은 어디까지 움직일건가
         * _wavevolum = 출렁이는 강약 1은 엄청 출렁거리며 값이 높아질수록 덜 출렁거린다
         * _overturn = 역으로 움직일건가
         * onFinish = 연출이 끝나고 부를 함수
         */

        Vector3 OriginPos = DirectionTarget.transform.localPosition;

        if (onStart != null)
            onStart();

        if (DirectionTarget == null)
            yield break;

        float startTime = _startTime;
        float endTime = _endTime;
        float timegap = endTime - startTime;

        float goalPos = 0.0f;
        float movetimer = 0.0f;

        float gradient = _gradient;

        float PlayTime = Time.time + endTime;
        float StartPlayTime = Time.time;

        while (PlayTime >= Time.time)
        {
            if (DirectionTarget == null)
                yield break;

            movetimer = Time.time - StartPlayTime;

            if (movetimer < startTime)
                yield return null;

            float childWaveVolum = 1.0f;
            float parWaveVolum = 1.0f;
            float backWaveVolum = 1.0f;

            if (_overturn == false)
            {
                for (int i = 0; i < _wavevolum; ++i)
                {
                    childWaveVolum *= (timegap - (movetimer - startTime));
                    parWaveVolum *= timegap;
                }
                backWaveVolum = ((timegap - (movetimer - startTime)) / timegap);
            }
            else
            {
                for (int i = 0; i < _wavevolum; ++i)
                {
                    childWaveVolum *= timegap - (timegap - (movetimer - startTime));
                    parWaveVolum *= timegap;
                }
                backWaveVolum = ((timegap - (timegap - (movetimer - startTime))) / timegap);
            }

            goalPos = (((childWaveVolum) / (parWaveVolum)) * MathDatas.Cos((backWaveVolum * 360.0f))) * gradient;

            Vector3 movepos = OriginPos;

            if (_eDrection == eMoveDirection.eMD_Left)
            {
                movepos.x -= goalPos;
            }
            else if (_eDrection == eMoveDirection.eMD_Right)
            {
                movepos.x += goalPos;
            }
            else if (_eDrection == eMoveDirection.eMD_Up)
            {
                movepos.y += goalPos;
            }
            else if (_eDrection == eMoveDirection.eMD_Down)
            {
                movepos.y -= goalPos;
            }

            if(DirectionTarget != null)
                DirectionTarget.transform.localPosition = movepos;

            yield return null;
        }


        if (DirectionTarget == null)
            yield break;

        DirectionTarget.transform.localPosition = OriginPos;

        if (onFinish != null)
            onFinish();
    }

    public void PlayNormalMoveDrection(Transform target, eMoveDirection _eDrection, Vector3 endPos, float startPos, float delayTime, float playtime, float duration, bool _overturn, System.Action onStart = null, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_positioncoroutine, target);

        _positioncoroutine.Add(target, StartCoroutine(PlayNormalMoveDrection(target, _eDrection, endPos, startPos, delayTime, playtime, duration, _overturn, onStart, onFinish)));
    }

    IEnumerator PlayNormalMoveDrection(Transform target, eMoveDirection _eDrection, Vector3 endPos, float startPos, float delayTime, float fplaytime, float duration, bool _overturn, System.Action onStart = null, System.Action onFinish = null)
    {
        /*
         * DirectionTarget = 스케일 조절할 타겟
         * _eDrection = 어느 방향에서 시작할지.
         * endPos = 본래 위치
         * startPos = 현재 위치
         * delayTime = 딜레이.
         * fplaytime = 몇초동안
         * duration = 아직 미적용
         * _overturn = 역방향.    true = +값  false = -값.
         */

        yield return new WaitForSeconds(delayTime);

        if (onStart != null)
            onStart();

        float playtime = fplaytime;
        float startTime = Time.time;
        float endTime = startTime + playtime;
        //스타트 위치.
        Vector3 vecstartPos = new Vector3(0.0f, 0.0f, 0.0f);

        
        Vector3 vectPos = new Vector3(0.0f, 0.0f, 0.0f);

        if (_eDrection == eMoveDirection.eMD_Left)
            vecstartPos = new Vector3(endPos.x - startPos, endPos.y, endPos.z);
        else if(_eDrection == eMoveDirection.eMD_Right)
            vecstartPos = new Vector3(endPos.x + startPos, endPos.y, endPos.z);
        else if(_eDrection == eMoveDirection.eMD_Up)
            vecstartPos = new Vector3(endPos.x, endPos.y + startPos, endPos.z);
        else if(_eDrection == eMoveDirection.eMD_Down)
            vecstartPos = new Vector3(endPos.x, endPos.y - startPos, endPos.z);

        while (endTime >=Time.time)
        {
            float direction = 0;

            if (_overturn == false)
            {
                if (_eDrection == eMoveDirection.eMD_Left)
                {
                    direction = vecstartPos.x + (startPos * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_Right)
                {
                    direction = vecstartPos.x - (startPos * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_Up)
                {
                    direction = vecstartPos.y - (startPos * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_Down)
                {
                    direction = vecstartPos.y + (startPos * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
            }
            else
            {
                if (_eDrection == eMoveDirection.eMD_Left)
                {
                    direction = endPos.x - (startPos * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_Right)
                {
                    direction = endPos.x + (startPos * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_Up)
                {
                    direction = endPos.y + (startPos * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_Down)
                {
                    direction = endPos.y - (startPos * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
            }

            if(_eDrection == eMoveDirection.eMD_Left || _eDrection == eMoveDirection.eMD_Right)
                vectPos = new Vector3(direction, vecstartPos.y, vecstartPos.z);
            else if (_eDrection == eMoveDirection.eMD_Up || _eDrection == eMoveDirection.eMD_Down)
                vectPos = new Vector3(vecstartPos.x, direction, vecstartPos.z);

            if(target != null)
                target.localPosition = vectPos;

            yield return null;
        }
        if (target == null)
            yield break;

        target.localPosition = endPos;

        if (onFinish != null)
            onFinish();
    }

    public void PlaySimpleScaleControl(Transform target, float _PlayTime, float _delayTime, Vector3 _GoalSize, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_scalecoroutine, target);

        _scalecoroutine.Add(target, StartCoroutine(PlaySimpleScale(target, _PlayTime, _delayTime, _GoalSize, onFinish)));
    }

    IEnumerator PlaySimpleScale(Transform target, float _PlayTime, float _delayTime, Vector3 _GoalSize, System.Action onFinish = null, bool removePrevDir = true)
    {
        /*
         * DirectionTarget = 스케일 조절할 타겟
         * _eScale = x,y,z
         * _PlayTime = 몇초동안
         * _MinSize = 최소 사이즈
         * _MaxSize = 최대 사이즈
         * _overturn = 역방향.    true = Sin  false = Cos
         */


        if (target == null)
            yield break;

        yield return new WaitForSeconds(_delayTime);

        Vector3 OriginScale = target.transform.localScale;
        Vector3 ScaleGap = _GoalSize - OriginScale;

        float playTime = _PlayTime;
        float startTime = Time.time;
        float endTime = startTime + playTime;


        while (endTime >= Time.time)
        {
            Vector3 fScale = OriginScale + (ScaleGap * MathDatas.Sin((90.0f * ((Time.time - startTime) / playTime))));
            target.transform.localScale = fScale;

            yield return null;
        }

        target.localScale = _GoalSize;

        if (onFinish != null)
            onFinish();
    }

    public void PlayScaleControl(Transform target, eScale _eScale, float _PlayTime, float _delayTime, float _MinSize, float _MaxSize, bool _overturn = false, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_scalecoroutine, target);

        _scalecoroutine.Add(target, StartCoroutine(PlayScale(target, _eScale, _PlayTime, _delayTime, _MinSize, _MaxSize, _overturn, onFinish)));
    }

    IEnumerator PlayScale(Transform DirectionTarget, eScale _eScale, float _PlayTime, float _delayTime, float _MinSize, float _MaxSize, bool _overturn = false, System.Action onFinish = null, bool removePrevDir = true)
    {
        /*
         * DirectionTarget = 스케일 조절할 타겟
         * _eScale = x,y,z
         * _PlayTime = 몇초동안
         * _MinSize = 최소 사이즈
         * _MaxSize = 최대 사이즈
         * _overturn = 역방향.    true = Sin  false = Cos
         */


        if (DirectionTarget == null)
            yield break;

        yield return new WaitForSeconds(_delayTime);

        Vector3 OriginScale = DirectionTarget.transform.localScale;

        if (_eScale == eScale.eScale_X)
        {
            Vector3 movepos = OriginScale;

            if (_overturn == false)
                movepos.x = _MinSize;
            else
                movepos.x = _MaxSize;
            DirectionTarget.transform.localScale = movepos;
        }
        else if (_eScale == eScale.eScale_Y)
        {
            Vector3 movepos = OriginScale;

            if (_overturn == false)
                movepos.y = _MinSize;
            else
                movepos.y = _MaxSize;
            DirectionTarget.transform.localScale = movepos;
        }
        else if (_eScale == eScale.eScale_Z)
        {
            Vector3 movepos = OriginScale;

            if (_overturn == false)
                movepos.z = _MinSize;
            else
                movepos.z = _MaxSize;
            DirectionTarget.transform.localScale = movepos;
        }
        else if( _eScale == eScale.eScale_XY)
        {
            Vector3 movepos = OriginScale;

            if (_overturn == false)
            {
                movepos.x = _MinSize;
                movepos.y = _MinSize;
            }
            else
            {
                movepos.x = _MaxSize;
                movepos.y = _MaxSize;
            }
            DirectionTarget.transform.localScale = movepos;
        }



        float playTime = _PlayTime;
        float startTime = Time.time;
        float endTime = startTime + playTime;

        while (endTime >= Time.time)
        {
            if (DirectionTarget == null)
                yield break;

            float fScale = 0.0f;

            if (_overturn == true)
                fScale = (MathDatas.Cos((90.0f * ((Time.time - startTime) / playTime))));
            else
                fScale = (MathDatas.Sin((90.0f * ((Time.time - startTime) / playTime))));

            Vector3 movepos = OriginScale;

            if (_eScale == eScale.eScale_X)
            {
                if (_overturn == false)
                    movepos.x = fScale * _MaxSize + _MinSize;
                else
                    movepos.x = fScale * _MinSize + _MaxSize;
            }
            else if (_eScale == eScale.eScale_Y)
            {
                if (_overturn == false)
                    movepos.y = fScale * _MaxSize + _MinSize;
                else
                    movepos.y = fScale * _MinSize + _MaxSize;
            }
            else if (_eScale == eScale.eScale_Z)
            {
                if (_overturn == false)
                    movepos.z = fScale * _MaxSize + _MinSize;
                else
                    movepos.z = fScale * _MinSize + _MaxSize;
            }

            //추가한 스케일 조절. 강조시에 이용 
            if (_eScale == eScale.eScale_XY)
            {
                float Scale = 0.0f;
                if (_overturn == false)
                    Scale = fScale * _MaxSize + _MinSize;
                else
                    Scale = fScale * _MinSize + _MaxSize;
                movepos.x = Scale;
                movepos.y = Scale;
            }

            if(DirectionTarget != null)
                DirectionTarget.localScale = movepos;

            yield return null;
        }

        if (DirectionTarget == null)
            yield break;

        DirectionTarget.localScale = OriginScale;

        if (onFinish != null)
            onFinish();
    }

    public void PlaySizeDeltaControl(Image target, eSizeDelta _eSizeDelta, float _playTime, float _Size, float _MaxSize, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveimageCoroutine(_masksizedeltacoroutine, target);

        _masksizedeltacoroutine.Add(target, StartCoroutine(PlaySizeDelta(target, _eSizeDelta, _playTime, _Size, _MaxSize, onFinish)));
    }

    IEnumerator PlaySizeDelta(Image target, eSizeDelta _eSizeDelta, float _playTime, float _Size, float _MaxSize, System.Action onFinish = null, bool removePrevDir = true)
    {

        float fPlayTime = _playTime;
        float fStartTime = Time.time;
        float fEndTime = fStartTime + fPlayTime;
        float fSize = 0.0f;
        Vector2 fOriSize = new Vector2(target.rectTransform.rect.width, target.rectTransform.rect.height);

        while(Time.time <= fEndTime)
        {

            float fPersent = 0;
            float fPersent2 = 0;

            if(_eSizeDelta == eSizeDelta.eSizeDelta_Y)
            {
                if (fSize < fOriSize.y)
                    fSize += _Size;
                else
                    fSize = fOriSize.y;

                fPersent = fSize / fOriSize.y * 100.0f;
                fPersent2 = _MaxSize * fPersent / 100.0f;

                target.RectTransform().sizeDelta = new Vector2(fOriSize.x, fPersent2);
                //target.rectTransform.sizeDelta = new Vector2(fOriSize.x, fPersent2);
            }
            else if(_eSizeDelta == eSizeDelta.eSizeDelta_X)
            {
                if (fSize < fOriSize.x)
                    fSize += _Size;
                else
                    fSize = fOriSize.x;

                fPersent = fSize / fOriSize.x * 100.0f;
                fPersent2 = _MaxSize * fPersent / 100.0f;

                target.RectTransform().sizeDelta = new Vector2(fPersent2, fOriSize.y);
                //target.rectTransform.sizeDelta = new Vector2(fPersent2, fOriSize.y);
            }

            yield return null;
        }


        if (target == null)
            yield break;

        target.RectTransform().sizeDelta = fOriSize;
        //target.rectTransform.sizeDelta = fOriSize;

        if (onFinish != null)
            onFinish();
    }

    public void PlayFillAmountControl(Image target, float _playTime, float _fspeed = 0.1f, bool _overturn = false, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveimageCoroutine(_fillamountcoroutine, target);

        _fillamountcoroutine.Add(target, StartCoroutine(PlayFillAmount(target, _playTime, _fspeed, _overturn, onFinish)));
    }

    IEnumerator PlayFillAmount(Image target, float _playTime, float _fspeed = 0.1f, bool _overturn = false, System.Action onFinish = null, bool removePrevDir = true)
    {
        /*
         * DirectionTarget = FillAmount 조절할 이미지
         * _PlayTime = 몇초동안
         * _fspeed = 얼마의 스피드로 조절을 할 것인지.
         * _overturn = 역방향.    true일 때 (+=)   false일 때 (-=)
         */

        float fPlayTime = _playTime;
        float fStartTime = Time.time;
        float fEndTime = fStartTime + fPlayTime;

        if (_overturn == false)
            target.fillAmount = 0;
        else
            target.fillAmount = 1;

        while (Time.time <= fEndTime)
        {
            if (target != null)
            {
                if (_overturn == false)
                {
                    if (target.fillAmount < 1)
                        target.fillAmount += _fspeed;
                    else
                        target.fillAmount = 1.0f;
                }
                else
                {
                    if (target.fillAmount > 0)
                        target.fillAmount -= _fspeed;
                    else
                        target.fillAmount = 0.0f;
                }
            }

            yield return null;
        }

        if (target == null)
            yield break;

        if (_overturn == false)
            target.fillAmount = 1.0f;
        else
            target.fillAmount = 0.0f;

        if (onFinish != null)
            onFinish();
    }

    public void PlayFillAmountGaugeControl(Image target, float _playTime, float _fgauge, float startgauge = 0.0f, float _firstPlayTime = 0.0f, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveimageCoroutine(_fillamountcoroutine, target);

        _fillamountcoroutine.Add(target, StartCoroutine(PlayFillAmountGauge(target, _playTime, _fgauge, startgauge, _firstPlayTime, onFinish)));
    }

    IEnumerator PlayFillAmountGauge(Image target, float _playTime, float _fgauge, float startgauge, float _firstPlayTime = 0.0f, System.Action onFinish = null, bool removePrevDir = true)
    {
        /*
         * DirectionTarget = FillAmount 조절할 이미지
         * _PlayTime = 몇초동안 실행 할 것인지.
         * _fgauge = 현재 게이지 값.
         * startgauge = 최대 게이지가 되었을때의 값
         * _firstPlayTime = 처음 while문 돌릴 때의 값.
         */

        //게이지 맨끝까지 올리기.100일떄의 예외처리.
        float fPlayTime = _firstPlayTime;
        float fStartTime = Time.time;
        float fEndTime = fStartTime + fPlayTime;

        float prevGauge = target.fillAmount;
        float gaugeGap = startgauge - target.fillAmount;

        if (target.fillAmount != 1.0f && 1.0f == startgauge)
        {
            while (Time.time <= fEndTime)
            {
                float maxDirectionGap = gaugeGap * (MathDatas.Sin((90.0f * ((Time.time - fStartTime) / fPlayTime))));

                target.fillAmount = prevGauge + maxDirectionGap;
                yield return null;
            }

            target.fillAmount = 1.0f;
        }

        //게이지 작업
        fPlayTime = _playTime;
        fStartTime = Time.time;
        fEndTime = fStartTime + fPlayTime;

        prevGauge = target.fillAmount;
        gaugeGap = _fgauge - target.fillAmount;

        while (Time.time <= fEndTime)
        {
            float direction = gaugeGap * (MathDatas.Sin((90.0f * ((Time.time - fStartTime) / fPlayTime))));

            target.fillAmount = prevGauge + direction;

            yield return null;
        }

        if (target == null)
            yield break;

        target.fillAmount = _fgauge;

        if (onFinish != null)
            onFinish();
    }

    public void PlayRotationControl(Transform _target, float _playTime, Vector3 _goalAngle, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (_target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_rotationcoroutine, _target);

        _rotationcoroutine.Add(_target, StartCoroutine(PlayRotation(_target, _playTime, _goalAngle, onFinish)));
    }

    IEnumerator PlayRotation(Transform _target, float _playTime, Vector3 _Angle, System.Action onFinish = null)
    {
        float fplayTime = _playTime;
        float fStartTime = Time.time;
        float fEndTime = fStartTime + fplayTime;

        Vector3 vecOriRotation = _target.transform.eulerAngles;

        Vector3 gabRotation = _Angle - vecOriRotation;

        while (Time.time <= fEndTime)
        {
            Vector3 fRotation;
            fRotation = vecOriRotation + (gabRotation * (MathDatas.Sin((90.0f * ((Time.time - fStartTime) / fplayTime)))));
            _target.transform.SetLocalRotationToEulerAngles(fRotation);
            yield return null;
        }

        if (_target == null)
            yield break;

        _target.transform.SetLocalRotationToEulerAngles(vecOriRotation);

        if (onFinish != null)
            onFinish();
    }
    
    public void ColorAlphaReset(Transform target, bool bInit, float fValue = 0.0f)
    {//알파값 초기화
        /*
         * target = 오브젝트 타겟
         * bInit = 투명도 1로 초기화할 지 0으로 초기화 할지 
         */

        List<Graphic> Childgrapic = target.GetComponentsInAllChildren<Graphic>();
        for (int i = 0; i < Childgrapic.Count; ++i)
        {
            Color initColor = Childgrapic[i].color;
            initColor.a = (bInit == true ? 1.0f : fValue);
            Childgrapic[i].color = initColor;
        }
    }

    public void ScaleReset(Transform target, bool bInit)
    {//스케일값 초기화
        /*
         * target = 오브젝트 타겟
         * bInit = 스케일값을 1로 초기화 할지 0으로 초기화 할지.
         */

        Vector3 vecScale;// = new Vector3(0.0f, 0.0f, 0.0f);
        if (bInit == false)
            vecScale = new Vector3(0.0f, 0.0f, 0.0f);
        else
            vecScale = new Vector3(1.0f, 1.0f, 1.0f);

        target.localScale = vecScale;
    }

    public void SetColor(Transform target, float fRGB1, float fRGB2, float fRGB3)
    {//컬러값 조절
        /*
         * target = 오브젝트 타겟
         * fRGB1 = R값
         * fRGB2 = G값
         * fRGB3 = B값
         */

        List<Graphic> childgrapic = target.GetComponentsInAllChildren<Graphic>();

        for (int i = 0; i < childgrapic.Count; ++i)
        {
            Color InitColor = childgrapic[i].color;

            float fR = fRGB1 / 255.0f;
            float fG = fRGB2 / 255.0f;
            float fB = fRGB3 / 255.0f;
            InitColor = new Color(fR, fG, fB);//녹색

            childgrapic[i].color = InitColor;
        }
    }

    //피벗이 중앙에 있으면 피벗값을 바꿔서 스케일을 키워준다. 그후 다시 처리하면?
    public void PivotScale(Transform target, Vector3 oriTestValue, float fScale)
    {
        /*
         * target = 오브젝트 or UI오브젝트
         * oriTestValue = UI시작시 로컬 위치
         * fScale = 넣을 스케일 값.
         */

        Vector2 vecOriPivot = ((RectTransform)target).pivot;//현재 피벗 값
        Vector3 vecOriScale = target.localScale;            //현재 스케일값.

        //위치값 변경
        target.localScale = new Vector3(fScale, vecOriScale.y, vecOriScale.z);

        float fWidthValue = ((RectTransform)target).rect.width / 2 * (target.localScale.x - 1);     //X에 더하거나 뺄때 이용
        float fHeightValue = ((RectTransform)target).rect.height / 2 * (target.localScale.y - 1);   //Y에 더하거나 뺄때 이용

        if (vecOriPivot.x == 0.0f)
        {//좌측정렬일때
            float fPosX = oriTestValue.x - fWidthValue;

            target.localPosition = new Vector3(fPosX, oriTestValue.y, oriTestValue.z);
        }
        else if (vecOriPivot.x == 0.5f)
        {//중앙정렬일때
        }
        else if (vecOriPivot.x == 1.0f)
        {//우측정렬일때
            float fPosX = oriTestValue.x + fWidthValue;

            target.localPosition = new Vector3(fPosX, oriTestValue.y, oriTestValue.z);
        }
    }
    public void AlignmentScale(Transform _target, float _fScale)
    {
        /*
         * target = 오브젝트 or UI오브젝트
         * fScale = 넣을 스케일 값.
         */

        Vector3 vecOriScale = _target.localScale;            //현재 스케일값.

        //위치값 변경
        _target.localScale = new Vector3(_fScale, _fScale, vecOriScale.z);

        float fWidthValue = ((RectTransform)_target).rect.width / 2 * (_target.localScale.x - 1);     //X에 더하거나 뺄때 이용
        float fHeightValue = ((RectTransform)_target).rect.height / 2 * (_target.localScale.y - 1);   //Y에 더하거나 뺄때 이용

        //정렬에 따라 글자 중앙의 포지션값을 빼주어야함.
        float fpreferredWidthValue = _target.GetComponent<LetterSpacing>().preferredWidth * (_target.localScale.x - 1) / 2;
        float fpreferredHeightValue = _target.GetComponent<LetterSpacing>().preferredHeight * (_target.localScale.y - 1) / 2;

        //(사이즈 델타/ 2) - (글자 간격 / 2) 중앙 값이 나온다.
        Vector2 fSizeDelta = _target.GetComponent<RectTransform>().sizeDelta;
        float fpreferredWidth = _target.GetComponent<LetterSpacing>().preferredWidth / 2;
        float fpreferredHeight = _target.GetComponent<LetterSpacing>().preferredHeight / 2;

        float fPosxValue = (fSizeDelta.x * 0.5f) - fpreferredWidth;
        float fPosyValue = (fSizeDelta.y * 0.5f) - fpreferredHeight;

        //어떤 정렬 인지.
        TextAnchor eAnchor = _target.GetComponent<Text>().alignment;

        float fPosX = 0.0f;
        float fPosY = 0.0f;

        switch (eAnchor)
        {
            case TextAnchor.MiddleLeft:     //중앙 좌측 정렬
                fPosX = fPosxValue - (-fWidthValue) - fpreferredWidthValue;
                //float fPosY = (-fPosyValue) + (-fHeightValue) + fpreferredHeightValue;
                _target.localPosition = new Vector3(fPosX, 0.0f, 0.0f);
                break;
            case TextAnchor.MiddleCenter:   //중앙 중앙 정렬
                _target.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            case TextAnchor.MiddleRight:    //중앙 우측 정렬
                fPosX = (-fPosxValue) + (-fWidthValue) + fpreferredWidthValue;
                //float fPosY = (-fPosyValue) + (-fHeightValue) + fpreferredHeightValue;
                _target.localPosition = new Vector3(fPosX, 0.0f, 0.0f);
                break;
            case TextAnchor.UpperLeft:      //위 좌측 정렬
                fPosX = fPosxValue - (-fWidthValue) - fpreferredWidthValue;
                fPosY = (-fPosyValue) + (-fHeightValue) + fpreferredHeightValue;

                _target.localPosition = new Vector3(fPosX, fPosY, 0.0f);
                break;
            case TextAnchor.UpperCenter:    //위 중앙 정렬
                //fPosX = fPosxValue - (-fWidthValue) - fpreferredWidthValue;
                fPosY = (-fPosyValue) + (-fHeightValue) + fpreferredHeightValue;

                _target.localPosition = new Vector3(0.0f, fPosY, 0.0f);
                break;
            case TextAnchor.UpperRight:     //위 우측 정렬
                fPosX = (-fPosxValue) + (-fWidthValue) + fpreferredWidthValue;
                fPosY = (-fPosyValue) + (-fHeightValue) + fpreferredHeightValue;

                _target.localPosition = new Vector3(fPosX, fPosY, 0.0f);
                break;

            case TextAnchor.LowerLeft:      //아래 좌측 정렬
                fPosX = fPosxValue - (-fWidthValue) - fpreferredWidthValue;
                fPosY = (fPosyValue) + (fHeightValue) - fpreferredHeightValue;

                _target.localPosition = new Vector3(fPosX, fPosY, 0.0f);
                break;

            case TextAnchor.LowerCenter:    //아래 중앙 정렬
                //fPosX = fPosxValue - (-fWidthValue) - fpreferredWidthValue;
                fPosY = (fPosyValue) + (fHeightValue) - fpreferredHeightValue;

                _target.localPosition = new Vector3(0.0f, fPosY, 0.0f);
                break;
            case TextAnchor.LowerRight:    //아래 우측 정렬
                fPosX = (-fPosxValue) + (-fWidthValue) + fpreferredWidthValue;
                fPosY = (fPosyValue) + (fHeightValue) - fpreferredHeightValue;

                _target.localPosition = new Vector3(fPosX, fPosY, 0.0f);
                break;

        }
    }

    public void PlayDiagonalMoveDrection(Transform target, eMoveDirection _eDrection, Vector3 endPos, Vector3 startPos, float delayTime, float playtime, float duration, bool _overturn, System.Action onStart = null, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_positioncoroutine, target);

        _positioncoroutine.Add(target, StartCoroutine(PlayDiagonalMoveDrection(target, _eDrection, endPos, startPos, delayTime, playtime, duration, _overturn, onStart, onFinish)));
    }

    IEnumerator PlayDiagonalMoveDrection(Transform target, eMoveDirection _eDrection, Vector3 endPos, Vector3 startPos, float delayTime, float fplaytime, float duration, bool _overturn, System.Action onStart = null, System.Action onFinish = null)
    {
        /*
         * DirectionTarget = 스케일 조절할 타겟
         * _eDrection = 어느 방향에서 시작할지.
         * endPos = 본래 위치
         * startPos = 시작 위치
         * delayTime = 딜레이.
         * fplaytime = 몇초동안
         * duration = 아직 미적용
         * _overturn = 역방향.    true = +값  false = -값.
         */

        yield return new WaitForSeconds(delayTime);

        if (onStart != null)
            onStart();

        float playtime = fplaytime;
        float startTime = Time.time;
        float endTime = startTime + playtime;
        //스타트 위치.
        Vector3 vecstartPos = new Vector3(0.0f, 0.0f, 0.0f);


        Vector3 vectPos = new Vector3(0.0f, 0.0f, 0.0f);

        if (_eDrection == eMoveDirection.eMD_LeftUp)
            vecstartPos = new Vector3(endPos.x - startPos.x, endPos.y + startPos.y, endPos.z + startPos.z);
        else if (_eDrection == eMoveDirection.eMD_LeftDown)
            vecstartPos = new Vector3(endPos.x - startPos.x, endPos.y - startPos.y, endPos.z + startPos.z);
        else if (_eDrection == eMoveDirection.eMD_RightUP)
            vecstartPos = new Vector3(endPos.x + startPos.x, endPos.y + startPos.y, endPos.z + startPos.z);
        else if (_eDrection == eMoveDirection.eMD_RightDown)
            vecstartPos = new Vector3(endPos.x + startPos.x, endPos.y - startPos.y, endPos.z + startPos.z);

        while (endTime >= Time.time)
        {
            float directionX = 0;
            float directionY = 0;

            if (_overturn == false)
            {
                if (_eDrection == eMoveDirection.eMD_LeftUp)
                {
                    directionX = vecstartPos.x + (startPos.x * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                    directionY = vecstartPos.y - (startPos.y * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_LeftDown)
                {
                    directionX = vecstartPos.x + (startPos.x * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                    directionY = vecstartPos.y + (startPos.y * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_RightUP)
                {
                    directionX = vecstartPos.x - (startPos.x * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                    directionY = vecstartPos.y - (startPos.y * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_RightDown)
                {
                    directionX = vecstartPos.x - (startPos.x * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                    directionY = vecstartPos.y + (startPos.y * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
            }
            else
            {
                if (_eDrection == eMoveDirection.eMD_LeftUp)
                {
                    directionX = endPos.x - (startPos.x * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                    directionY = endPos.y + (startPos.y * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_LeftDown)
                {
                    directionX = endPos.x - (startPos.x * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                    directionY = endPos.y - (startPos.y * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_RightUP)
                {
                    directionX = endPos.x + (startPos.x * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                    directionY = endPos.y + (startPos.y * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
                else if (_eDrection == eMoveDirection.eMD_RightDown)
                {
                    directionX = endPos.x + (startPos.x * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                    directionY = endPos.y - (startPos.y * (MathDatas.Sin((90.0f * (Time.time - startTime) / playtime))));
                }
            }

            vectPos = new Vector3(directionX, directionY, vecstartPos.z);

            if (target != null)
                target.localPosition = vectPos;

            yield return null;
        }
        if (target == null)
            yield break;

        target.localPosition = endPos;

        if (onFinish != null)
            onFinish();
    }

    public void PlayScaleControlTest(Transform target, eScale _eScale, float _PlayTime, float _delayTime, float _MinSize, float _MaxSize, bool _overturn = false, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveCoroutine(_scalecoroutine, target);

        _scalecoroutine.Add(target, StartCoroutine(PlayScaletest(target, _eScale, _PlayTime, _delayTime, _MinSize, _MaxSize, _overturn, onFinish)));
    }

    IEnumerator PlayScaletest(Transform DirectionTarget, eScale _eScale, float _PlayTime, float _delayTime, float _MinSize, float _MaxSize, bool _overturn = false, System.Action onFinish = null, bool removePrevDir = true)
    {
        /*
         * DirectionTarget = 스케일 조절할 타겟
         * _eScale = x,y,z
         * _PlayTime = 몇초동안
         * _MinSize = 최소 사이즈
         * _MaxSize = 최대 사이즈
         * _overturn = 역방향.    true = Sin  false = Cos
         */
        yield return new WaitForSeconds(_delayTime);

        Vector3 OriginScale = DirectionTarget.transform.localScale;

        if (DirectionTarget == null)
            yield break;
        
        //여기는 커지거나 작아지는 구간.
        float playTime = _PlayTime;
        float startTime = Time.time;
        float endTime = startTime + playTime;

        while (endTime >= Time.time)
        {
            if (DirectionTarget == null)
                yield break;

            float fScale = 0.0f;

            if (_overturn == true)
                fScale = (MathDatas.Cos((90.0f * ((Time.time - startTime) / playTime))));
            else
                fScale = (MathDatas.Sin((90.0f * ((Time.time - startTime) / playTime))));

            Vector3 movepos = OriginScale;

            if (_eScale == eScale.eScale_X)
            {
                if (_overturn == false)
                    movepos.x = fScale * _MaxSize + _MinSize;
                else
                    movepos.x = fScale * _MinSize;
            }
            else if (_eScale == eScale.eScale_Y)
            {
                if (_overturn == false)
                    movepos.y = fScale * _MaxSize + _MinSize;
                else
                    movepos.y = fScale * _MinSize;
            }
            else if (_eScale == eScale.eScale_Z)
            {
                if (_overturn == false)
                    movepos.z = fScale * _MaxSize + _MinSize;
                else
                    movepos.z = fScale * _MinSize;
            }

            //추가한 스케일 조절. 강조시에 이용 
            if (_eScale == eScale.eScale_XY)
            {
                float Scale = 0.0f;
                if (_overturn == false)
                    Scale = fScale * _MaxSize + _MinSize;
                else
                    Scale = fScale * _MinSize;
                movepos.x = Scale;
                movepos.y = Scale;
            }

            if (DirectionTarget != null)
                DirectionTarget.localScale = movepos;

            yield return null;
        }

        //여기는 커지거나 작아지는 구간.
        playTime = _PlayTime;
        startTime = Time.time;
        endTime = startTime + playTime;

        while (endTime >= Time.time)
        {
            if (DirectionTarget == null)
                yield break;

            float fScale = 0.0f;

            if (_overturn == true)
                fScale = (MathDatas.Sin((90.0f * ((Time.time - startTime) / playTime))));
            else
                fScale = (MathDatas.Cos((90.0f * ((Time.time - startTime) / playTime))));

            Vector3 movepos = OriginScale;

            if (_eScale == eScale.eScale_X)
            {
                if (_overturn == false)
                    movepos.x = fScale * _MaxSize + _MinSize;
                else
                    movepos.x = fScale * _MinSize;
            }
            else if (_eScale == eScale.eScale_Y)
            {
                if (_overturn == false)
                    movepos.y = fScale * _MaxSize + _MinSize;
                else
                    movepos.y = fScale * _MinSize;
            }
            else if (_eScale == eScale.eScale_Z)
            {
                if (_overturn == false)
                    movepos.z = fScale * _MaxSize + _MinSize;
                else
                    movepos.z = fScale * _MinSize;
            }

            //추가한 스케일 조절. 강조시에 이용 
            if (_eScale == eScale.eScale_XY)
            {
                float Scale = 0.0f;
                if (_overturn == false)
                    Scale = fScale * _MaxSize + _MinSize;
                else
                    Scale = fScale * _MinSize;
                movepos.x = Scale;
                movepos.y = Scale;
            }

            if (DirectionTarget != null)
                DirectionTarget.localScale = movepos;

            yield return null;
        }

        if (DirectionTarget == null)
            yield break;

        DirectionTarget.localScale = OriginScale;

        if (onFinish != null)
            onFinish();
    }

    public void PlayImageSwitchControl(Image target, Sprite sprite1, Sprite sprite2, float _cycleTime, bool _loop = false, System.Action onFinish = null, bool removePrevDir = true)
    {
        if (target == null)
            return;

        if (removePrevDir)
            RemoveimageCoroutine(_imageswitchcoroutine, target);

        _imageswitchcoroutine.Add(target, StartCoroutine(PlayImageSwitch(target, sprite1, sprite2, _cycleTime, _loop, onFinish)));
    }

    public void RemoveSwitchControl(Image target)
    {
        if (target == null)
            return;

        RemoveimageCoroutine(_imageswitchcoroutine, target);
    }

    IEnumerator PlayImageSwitch(Image target, Sprite sprite1, Sprite sprite2, float _cycleTime, bool _loop = false, System.Action onFinish = null)
    {
        if (target == null)
            yield break;

        target.sprite = sprite1;

        float playTime = _cycleTime;
        float startTime = Time.time;
        float endTime = startTime + playTime;

        bool changetype = false;

        do
        {
            startTime = Time.time;
            endTime = startTime + playTime;

            while (endTime >= Time.time)
                yield return null;

            if (target == null)
                yield break;

            target.sprite = changetype == false ? sprite2 : sprite1;
            changetype = !changetype;
        } while (_loop);


        if (target == null)
            yield break;

        if (onFinish != null)
            onFinish();
    }
}
