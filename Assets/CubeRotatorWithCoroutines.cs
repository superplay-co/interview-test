using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A script that rotates a cube 360 degrees and moves it up and down in a loop using Coroutines.
/// </summary>
public class CubeRotatorWithCoroutines : MonoBehaviour
{
    [SerializeField] private Transform _cube;
    [SerializeField] private float _rotationDuration;
    [SerializeField] private float _translationDuration;

    private Coroutine _rotationCoroutine;
    private Coroutine _translationCoroutine;

    private void Awake()
    {
        _rotationCoroutine = StartCoroutine(RotateCubeLoop());
        _translationCoroutine = StartCoroutine(DoTranslationLoop());
    }

    private void OnDestroy()
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
        }

        if (_translationCoroutine != null)
        {
            StopCoroutine(_translationCoroutine);
        }
    }

    private IEnumerator RotateCubeLoop()
    {
        while (true)
        {
            var coroutine = Do360Rotation(_rotationDuration);
            while (coroutine.MoveNext() == true)
                yield return null;
        }

        Debug.Log("Rotation cancelled");
    }

    private IEnumerator Do360Rotation(float duration)
    {
        var startTime = Time.time;
        var initialYRotation = _cube.rotation.eulerAngles.y;

        while (Time.time - startTime < duration)
        {
            var currentAnimationProgress = (Time.time - startTime) / duration;
            var currentRotation = Mathf.Lerp(initialYRotation, initialYRotation + 360, currentAnimationProgress);
            _cube.Rotate(Vector3.up, currentRotation);

            yield return null;
        }
    }

    private IEnumerator DoTranslationLoop()
    {
        while (true)
        {
            yield return StartCoroutine(DoTranslation(_translationDuration, Vector3.up));
            yield return StartCoroutine(DoTranslation(_translationDuration, Vector3.down));
        }
    }

    private IEnumerator DoTranslation(float duration, Vector3 direction)
    {
        var startTime = Time.time;
        var initialPosition = _cube.position;
        var targetPosition = _cube.position + direction * 2;
        var currentAnimationProgress = 0.0f;
        while (Time.time - startTime < duration)
        {
            currentAnimationProgress += (Time.time - startTime) / duration;
            _cube.position = Vector3.Lerp(initialPosition, targetPosition, currentAnimationProgress);

            yield return null;
        }

        // Ensure the cube reaches the target position to avoid drift over time
        _cube.position = targetPosition;
    }
}
