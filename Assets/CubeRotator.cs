using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A script that rotates a cube 360 degrees and moves it up and down in a loop.
/// </summary>
public class CubeRotator : MonoBehaviour
{
    [SerializeField] private Transform _cube;
    [SerializeField] private float _rotationDuration;
    [SerializeField] private float _translationDuration;

    private void Awake()
    {
        RotateCube(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();

        DoTranslationLoop(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
    }

    private async UniTask RotateCube(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            await Do360Rotation(_rotationDuration, cancellationToken);
        }
    }

    private async UniTask Do360Rotation(float duration, CancellationToken cancellationToken)
    {
        var startTime = Time.time;
        var initialYRotation = _cube.rotation.eulerAngles.y;
        while (Time.time - startTime < duration)
        {
            var currentAnimationProgress = (Time.time - startTime) / duration;
            var currentRotation = Mathf.Lerp(initialYRotation, initialYRotation + 360, currentAnimationProgress);
            _cube.Rotate(Vector3.up, currentRotation);

            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
    }

    private async UniTask DoTranslationLoop(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            await DoTranslation(_translationDuration, Vector3.up, destroyCancellationToken);
            await DoTranslation(_translationDuration, Vector3.down, destroyCancellationToken);
        }
    }

    private async UniTask DoTranslation(float duration, Vector3 direction, CancellationToken cancellationToken)
    {
        var startTime = Time.time;
        var initialPosition = _cube.position;
        var targetPosition = _cube.position + direction * 2;
        var currentAnimationProgress = 0f;
        while (Time.time - startTime < duration)
        {
            currentAnimationProgress += (Time.time - startTime) / duration;
            _cube.position = Vector3.Lerp(initialPosition, targetPosition, currentAnimationProgress);

            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
    }
}
