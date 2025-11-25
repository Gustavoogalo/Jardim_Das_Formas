using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Selector.MVVM_Selector;
using UnityEngine.UI;

namespace Mechanics.Selector.Selector
{
    [System.Serializable]
    public class IconData
    {
        public IconType Type;
        public IconColor Color;
        public IconSize Size;

        public IconData(IconType type, IconColor color, IconSize size)
        {
            Type = type;
            Color = color;
            Size = size;
        }

        public bool IsEqual(IconData other)
        {
            return Type == other.Type && Color == other.Color && Size == other.Size;
        }
    }

    public class ChallengeSelector : MonoBehaviour
    {
        [Header("Sprite Mapping")]
        [SerializeField] private IconSpriteMapper spriteMapper;

        [Header("Prefabs")] [SerializeField] private FormIconView iconPrefab;
        [SerializeField] private Button answerButtonPrefab;

        [Header("Containers")] [SerializeField]
        private Transform referencePanel;

        [SerializeField] private Transform[] answerButtonContainers;

        [Header("Game Parameters")] [SerializeField]
        private int minReferenceIcons = 3;

        [SerializeField] private int maxReferenceIcons = 4;
        [SerializeField] private int answerIconCount = 5;
        [SerializeField] private int totalButtons = 4;

        private List<IconData> correctSequence;
        private int correctButtonIndex;
        private List<IconData> allValidIconData; 

        void Awake()
        {
            if (spriteMapper != null)
            {
                allValidIconData = spriteMapper.GetAllValidIconData(); 
            }
            
            if (allValidIconData == null || allValidIconData.Count == 0)
            {
                Debug.LogError("SpriteMapper não forneceu nenhuma combinação IconData válida. Desativando ChallengeSelector.");
                enabled = false;
            }
        }

        void Start()
        {
            if (enabled)
            {
                SetupNewChallenge();
            }
        }

        #region Sequence Mechanic
        private IconData GetRandomValidIconData()
        {
            if (allValidIconData == null || allValidIconData.Count == 0)
            {
                return null;
            }
            
            int randomIndex = Random.Range(0, allValidIconData.Count);
            IconData original = allValidIconData[randomIndex];
            return new IconData(original.Type, original.Color, original.Size);
        }

        public void SetupNewChallenge()
        {
            ClearContainers();

            int referenceCount = Random.Range(minReferenceIcons, maxReferenceIcons + 1);
            correctSequence = new List<IconData>();
            for (int i = 0; i < referenceCount; i++)
            {
                correctSequence.Add(GetRandomValidIconData());
            }

            InstantiateIcons(referencePanel, correctSequence);

            correctButtonIndex = Random.Range(0, totalButtons);

            for (int i = 0; i < totalButtons; i++)
            {
                if (answerButtonContainers == null || answerButtonContainers.Length <= i)
                {
                    Debug.LogError($"Container para o botão {i} está faltando.");
                    continue;
                }
                
                Transform buttonContainer = answerButtonContainers[i];

                List<IconData> answerSequence;

                if (i == correctButtonIndex)
                {
                    answerSequence = GenerateCorrectAnswerSequence(correctSequence, answerIconCount);
                }
                else
                {
                    answerSequence = GenerateIncorrectAnswerSequence(correctSequence, answerIconCount);
                }

                InstantiateIcons(buttonContainer, answerSequence);

                Button buttonComponent = buttonContainer.GetComponentInParent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.RemoveAllListeners();
                    int buttonId = i;
                    buttonComponent.onClick.AddListener(() => OnAnswerSelected(buttonId));
                }
            }
        }

        private List<IconData> GenerateCorrectAnswerSequence(List<IconData> correctRef, int totalIcons)
        {
            List<IconData> answer = new List<IconData>();

            int maxStartIndex = totalIcons - correctRef.Count;
            int startIndex = Random.Range(0, maxStartIndex + 1);

            for (int i = 0; i < totalIcons; i++)
            {
                if (i >= startIndex && i < startIndex + correctRef.Count)
                {
                    int refIndex = i - startIndex;
                    answer.Add(correctRef[refIndex]);
                }
                else
                {
                    answer.Add(GetRandomValidIconData());
                }
            }
            return answer;
        }

        private List<IconData> GenerateIncorrectAnswerSequence(List<IconData> correctRef, int totalIcons)
        {
            List<IconData> answer;
            bool isCorrect;
            int maxAttempts = 100;

            do
            {
                answer = new List<IconData>();
                for (int i = 0; i < totalIcons; i++)
                {
                    answer.Add(GetRandomValidIconData());
                }

                isCorrect = CheckIfSequenceContainsCorrectRef(answer, correctRef);

                maxAttempts--;
            } while (isCorrect && maxAttempts > 0);

            if (maxAttempts == 0 && isCorrect)
            {
                for (int i = 0; i <= totalIcons - correctRef.Count; i++)
                {
                    bool match = true;
                    for (int j = 0; j < correctRef.Count; j++)
                    {
                        if (!answer[i + j].IsEqual(correctRef[j]))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        IconType originalType = answer[i].Type;
                        IconType newType =
                            (IconType)(((int)originalType + 1) % System.Enum.GetValues(typeof(IconType)).Length);
                        answer[i].Type = newType;
                        break;
                    }
                }
            }

            return answer;
        }

        private bool CheckIfSequenceContainsCorrectRef(List<IconData> answerSequence, List<IconData> correctRef)
        {
            if (answerSequence.Count < correctRef.Count) return false;

            for (int i = 0; i <= answerSequence.Count - correctRef.Count; i++)
            {
                bool match = true;
                for (int j = 0; j < correctRef.Count; j++)
                {
                    if (!answerSequence[i + j].IsEqual(correctRef[j]))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return true;
                }
            }
            return false;
        }
        
        private void InstantiateIcons(Transform parent, List<IconData> sequence)
        {
            if (parent == null) return;
            
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }

            foreach (var data in sequence)
            {
                FormIconView newIcon = Instantiate(iconPrefab, parent);
                Sprite visualSprite = spriteMapper.GetSprite(data.Type, data.Color, data.Size);
                newIcon.Setup(data, visualSprite);
            }
        }

        private void OnAnswerSelected(int selectedButtonIndex)
        {
            if (selectedButtonIndex == correctButtonIndex)
            {
                Debug.Log("Resposta Correta! Avance para o próximo desafio.");
            }
            else
            {
                Debug.Log("Resposta Incorreta. Tente novamente ou penalize o jogador.");
            }
        }
        
        private void ClearContainers()
        {
            if (referencePanel != null)
            {
                ClearChildren(referencePanel);
            }
    
            if (answerButtonContainers != null)
            {
                foreach (var container in answerButtonContainers)
                {
                    if (container != null)
                    {
                        ClearChildren(container);
                    }
                }
            }
        }

        private void ClearChildren(Transform parent)
        {
            List<GameObject> childrenToDestroy = new List<GameObject>();
            foreach (Transform child in parent)
            {
                childrenToDestroy.Add(child.gameObject);
            }

            foreach (GameObject child in childrenToDestroy)
            {
                Destroy(child);
            }
        }
        #endregion
        
        #region Timer And Stars Mechanic
        #endregion
        
        #region Events Mechanic
        #endregion
    }
}