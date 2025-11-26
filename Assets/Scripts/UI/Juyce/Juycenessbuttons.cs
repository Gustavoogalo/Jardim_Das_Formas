using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Juycenessbuttons : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private struct ChildState
    {
        public Transform transform;
        public Vector3 initialScale;
        public Vector3 initialPosition;
    }

    public bool isChallengeSelector = false;
    [SerializeField] private float timeBetweenCharacters = 0.05f;
    [SerializeField] private float scaleFactor = 1.15f;
    [SerializeField] private float moveAmountY = 10f;
    [SerializeField] private float animationDuration = 0.1f;

    public HorizontalLayoutGroup layoutGroupToDisable;

    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Coroutine activeCoroutine;
    private bool isHovering = false;
    private ChildState[] childrenStates;

    void Awake()
    {
        if (layoutGroupToDisable == null)
        {
            layoutGroupToDisable = transform.parent?.GetComponent<HorizontalLayoutGroup>();
        }

        initialScale = transform.localScale;
        initialPosition = transform.localPosition;
    }

    private void Animate(bool enter)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        isHovering = enter;

        if (enter)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);
            Transform[] tempChildren;
            if (allChildren.Length > 0)
            {
                tempChildren = new Transform[allChildren.Length - 1];
                System.Array.Copy(allChildren, 1, tempChildren, 0, allChildren.Length - 1);
            }
            else
            {
                tempChildren = new Transform[0];
            }

            childrenStates = new ChildState[tempChildren.Length];
            for (int i = 0; i < tempChildren.Length; i++)
            {
                childrenStates[i] = new ChildState
                {
                    transform = tempChildren[i],
                    initialScale = tempChildren[i].localScale,
                    initialPosition = tempChildren[i].localPosition
                };
            }

            activeCoroutine = StartCoroutine(DoEnterAnimation());
        }
        else
        {
            activeCoroutine = StartCoroutine(DoExitAnimation());
        }
    }

    IEnumerator DoEnterAnimation()
    {
        if (layoutGroupToDisable != null)
        {
            layoutGroupToDisable.enabled = false;
        }

        yield return null;

        if (isChallengeSelector && childrenStates != null && childrenStates.Length > 0)
        {
            foreach (var state in childrenStates)
            {
                if (state.transform == null) continue;

                StartCoroutine(LerpTransform(state.transform, state.initialScale * scaleFactor,
                    state.initialPosition + Vector3.up * moveAmountY / 2f, animationDuration));
                yield return new WaitForSeconds(timeBetweenCharacters);
            }

            yield return new WaitForSeconds(animationDuration);

            foreach (var state in childrenStates)
            {
                if (state.transform == null) continue;

                StartCoroutine(LerpTransform(state.transform, state.initialScale,
                    state.initialPosition, animationDuration));
            }

            StartCoroutine(LerpTransform(transform, initialScale * scaleFactor,
                initialPosition + Vector3.up * moveAmountY, animationDuration));
        }
        else
        {
            yield return StartCoroutine(LerpTransform(transform, initialScale * scaleFactor,
                initialPosition + Vector3.up * moveAmountY, animationDuration));
        }
    }

    IEnumerator DoExitAnimation()
    {
        yield return StartCoroutine(LerpTransform(transform, initialScale, initialPosition, animationDuration));

        if (isChallengeSelector && childrenStates != null && childrenStates.Length > 0)
        {
            foreach (var state in childrenStates)
            {
                if (state.transform == null) continue;

                StartCoroutine(LerpTransform(state.transform, state.initialScale, state.initialPosition, animationDuration));
            }

            yield return new WaitForSeconds(animationDuration);
        }

        transform.localScale = initialScale;
        transform.localPosition = initialPosition;

        if (layoutGroupToDisable != null)
        {
            layoutGroupToDisable.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroupToDisable.transform);
        }
    }

    IEnumerator LerpTransform(Transform target, Vector3 finalScale, Vector3 finalLocalPosition, float duration)
    {
        Vector3 startScale = target.localScale;
        Vector3 startLocalPosition = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(startScale, finalScale, t);
            target.localPosition = Vector3.Lerp(startLocalPosition, finalLocalPosition, t);

            yield return null;
        }

        target.localScale = finalScale;
        target.localPosition = finalLocalPosition;
    }

    public void OnSelect(BaseEventData eventData) => Animate(true);
    public void OnPointerEnter(PointerEventData eventData) => Animate(true);
    public void OnPointerExit(PointerEventData eventData) => Animate(false);
}