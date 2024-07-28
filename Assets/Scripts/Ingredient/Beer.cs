using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.ParticleSystem;

public class Beer : MonoBehaviour
{
    [SerializeField] private Vector3 _slideStartPosition = new Vector3(-11, -2.5f, -28);
    [SerializeField] private Vector3 _slideEndPosition = new Vector3(0, -2.5f, -28);

    [HideInInspector] private ClientManager _clientManager;

    private MeshRenderer _meshRenderer;
    private ParticleSystem _particles;

    public Stats Stats { get; private set; }

    private void Update()
    {
        
    }

    public void Slide()
    {
        transform.position = _slideStartPosition;
        transform.DOMove(_slideEndPosition, 2).SetEase(Ease.OutCirc);

        Utility.Delay(1, () => _particles.Stop());

        Utility.Delay(2.3f, () =>
        {
            var pos = _clientManager.GetCurrentClient().transform.position;
            transform.DOMove(new Vector3(pos.x, pos.y-1.2f, -28),0.5f).onComplete=
                ()=> 
                {
                    transform.DORotate(new Vector3(110, 0, 0), 2).SetEase(Ease.OutCirc).SetDelay(0.2f);
                    transform.DOMoveY(pos.y+0.7f,2).SetEase(Ease.OutCirc).SetDelay(0.2f);
                    _meshRenderer.transform.DOScaleY(0,2f).SetEase(Ease.OutCirc).SetDelay(0.3f)
                        .onComplete=()=> { _meshRenderer.transform.localScale = Vector3.zero; };
                };
        });
        Utility.Delay(5.7f, () =>
        {
            _clientManager.Confirm(Stats);

            transform.DORotate(new Vector3(0, 0, 0), 1).SetEase(Ease.InQuad);
            transform.DOMoveY(_slideEndPosition.y, 1).SetEase(Ease.InCirc).SetDelay(0.2f);

            Utility.Delay(5, () =>
            {
                transform.DOJump(new Vector3(10, -8, -28), 5, 1, 2).SetEase(Ease.OutCirc);
                transform.DORotate(new Vector3(0, 0, 360 * 6), 2, RotateMode.FastBeyond360).SetEase(Ease.OutCirc);
            });
        });
    }

    public void SetStats(Stats stats, ClientManager manager)
    {
        Stats = stats;
        _clientManager = manager;

        var color = GameManager.Instance.Colors.Array[stats.GetHighestStatIndex()];

        _particles = GetComponentInChildren<ParticleSystem>();
        //var main = _particles.main;
        //main.startColor = color;
        _particles.Play();

        _meshRenderer = GetComponentsInChildren<MeshRenderer>()[1];
        var mat = Instantiate(_meshRenderer.material);
        mat.color = color;
        _meshRenderer.material = mat;
    }
}
