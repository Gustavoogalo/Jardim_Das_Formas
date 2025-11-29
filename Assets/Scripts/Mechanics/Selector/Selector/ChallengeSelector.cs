using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mechanics.Selector.MVVM_Selector;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

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

        public bool IsEqual(IconData other, bool checkType, bool checkColor, bool checkSize)
        {
            bool typeMatch = !checkType || (Type == other.Type);
            bool colorMatch = !checkColor || (Color == other.Color);
            bool sizeMatch = !checkSize || (Size == other.Size);

            return typeMatch && colorMatch && sizeMatch;
        }

        public bool IsEqual(IconData other)
        {
            return Type == other.Type && Color == other.Color && Size == other.Size;
        }
    }

    public class ChallengeSelector : MonoBehaviour
    {
        [Header("Sprite Mapping")] [SerializeField]
        private IconSpriteMapper spriteMapper;

        [Header("Prefabs")] [SerializeField] private FormIconView iconPrefab;
        [SerializeField] private Button answerButtonPrefab;

        [Header("Containers")] [SerializeField]
        private Transform referencePanel;

        [SerializeField] private Transform[] answerButtonContainers;

        [Header("Game Parameters")] [SerializeField]
        private int minReferenceIcons = 3;

        [SerializeField] private int maxReferenceIcons = 4;
        [SerializeField] private int answerIconCount = 5;
        [SerializeField] private int totalButtons = 3;

        private List<IconData> correctSequence;
        private int correctButtonIndex;
        private List<IconData> allValidIconData;

        [Header("Timer Settings")] [SerializeField]
        private float challengeDuration = 60f;

        [SerializeField] private Slider challengeTimerSlider;

        [Header("Score and Star Settings")] [SerializeField]
        private StarInventory starInventory;

        [SerializeField] private int maxStars = 3;
        [SerializeField] private int initialScore = 100;
        [SerializeField] private int incorrectPenalty = 20;
        [SerializeField] [Range(0, 1)] private float timeThreshold3Stars = 0.5f;

        [Header("Dependências")] [SerializeField]
        private RewardScreenManager rewardManager;

        private float currentScore;
        private int wrongAttempts;
        private float timeRemaining;
        private bool challengeActive;

        public bool isType;
        public bool isColor;
        public bool isSize;
        
        private (bool type, bool color, bool size) ChallengeCriteria => (isType, isColor, isSize);
        
        private Action<bool> OnChallengeStarted;
        private Action<bool> OnChallengeEnded;
        
        public void SetChallengeCriteriaAndStart(bool type, bool color, bool size)
        {
            this.isType = type;
            this.isColor = color;
            this.isSize = size;
            
            // Chama o Setup para garantir que o pool de IconData (allValidIconData)
            // seja filtrado corretamente e o desafio comece.
            SetupNewChallenge();
        }
        
        
        private void OnEnable()
        {
            SetupNewChallenge();
        }

        void Awake()
        {
            if (spriteMapper == null)
            {
                Debug.LogError("SpriteMapper não atribuído.");
                enabled = false;
            }
        }

        private void Update()
        {
            if (challengeActive)
            {
                timeRemaining -= Time.deltaTime;

                if (challengeTimerSlider != null)
                {
                    challengeTimerSlider.value = timeRemaining / challengeDuration;
                }

                if (timeRemaining <= 0)
                {
                    challengeActive = false;
                    timeRemaining = 0;
                    OnChallengeEnded?.Invoke(false);
                    Debug.Log("Tempo esgotado! Tentar Novamente.");
                }
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
            if (spriteMapper != null)
            {
                allValidIconData = spriteMapper.GetAllValidIconData(isType, isColor, isSize);
            }
            
            if (allValidIconData == null || allValidIconData.Count == 0)
            {
                Debug.LogError(
                    "SpriteMapper não forneceu nenhuma combinação IconData válida. Verifique as configurações e os booleanos.");
                return;
            }

            ClearContainers();

            int referenceCount = Random.Range(minReferenceIcons, maxReferenceIcons + 1);
            correctSequence = new List<IconData>();
            for (int i = 0; i < referenceCount; i++)
            {
                correctSequence.Add(GetRandomValidIconData());
            }

            InstantiateIcons(referencePanel, correctSequence);

            challengeActive = true;
            timeRemaining = challengeDuration;
            currentScore = initialScore;
            wrongAttempts = 0;

            if (challengeTimerSlider != null)
            {
                challengeTimerSlider.maxValue = 1f;
                challengeTimerSlider.value = 1f;
            }

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
            var (checkType, checkColor, checkSize) = ChallengeCriteria;

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
                    if (CheckIfSequenceContainsCorrectRef(answer.GetRange(i, correctRef.Count), correctRef))
                    {
                        if (checkType)
                        {
                            IconType originalType = answer[i].Type;
                            IconType newType =
                                (IconType)(((int)originalType + 1) % System.Enum.GetValues(typeof(IconType)).Length);
                            answer[i].Type = newType;
                        }
                        else if (checkColor)
                        {
                            IconColor originalColor = answer[i].Color;
                            IconColor newColor =
                                (IconColor)(((int)originalColor + 1) % System.Enum.GetValues(typeof(IconColor)).Length);
                            answer[i].Color = newColor;
                        }
                        else if (checkSize)
                        {
                            IconSize originalSize = answer[i].Size;
                            IconSize newSize =
                                (IconSize)(((int)originalSize + 1) % System.Enum.GetValues(typeof(IconSize)).Length);
                            answer[i].Size = newSize;
                        }
                        break;
                    }
                }
            }

            return answer;
        }

        private bool CheckIfSequenceContainsCorrectRef(List<IconData> answerSequence, List<IconData> correctRef)
        {
            if (answerSequence.Count < correctRef.Count) return false;
            
            var (checkType, checkColor, checkSize) = ChallengeCriteria;

            for (int i = 0; i <= answerSequence.Count - correctRef.Count; i++)
            {
                bool match = true;
                for (int j = 0; j < correctRef.Count; j++)
                {
                    if (!answerSequence[i + j].IsEqual(correctRef[j], checkType, checkColor, checkSize))
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
            if (!challengeActive) return;

            if (selectedButtonIndex == correctButtonIndex)
            {
                RightAnswerSelected();
            }
            else
            {
                WrongAnsweerSelected();
            }
        }

        private void WrongAnsweerSelected()
        {
            wrongAttempts++;

            currentScore -= incorrectPenalty;

            if (currentScore < 0) currentScore = 0;


            Debug.Log($"Resposta Incorreta. Penalidade de {incorrectPenalty} pontos. Pontos atuais: {currentScore}");
        }

        private void RightAnswerSelected()
        {
            challengeActive = false;

            int stars = CalculateStars();

            if (starInventory != null)
            {
                starInventory.AddStars(stars);
                Game_Events.ChallengeCompleted(stars);
            }

            if (rewardManager != null)
            {
                rewardManager.gameObject.SetActive(true);
                rewardManager.ShowRewards(stars, gameObject);
            }
            else
            {
                OnChallengeEnded?.Invoke(true);
            }

            Debug.Log(
                $"Resposta Correta! Você ganhou {stars} estrela(s). Total acumulado: {starInventory.CurrentStars}");
        }

        private int CalculateStars()
        {
            float timeRatio = timeRemaining / challengeDuration;
            int starsFromScore;

            if (currentScore >= initialScore)
            {
                starsFromScore = 3;
            }
            else if (currentScore >= initialScore * 0.5f)
            {
                starsFromScore = 2;
            }
            else if (wrongAttempts >= totalButtons / 2)
            {
                starsFromScore = 1;
            }
            else
            {
                starsFromScore = 0;
            }

            int finalStars = Mathf.Max(1, starsFromScore);

            if (finalStars == maxStars && timeRatio < timeThreshold3Stars)
            {
                finalStars = Mathf.Max(1, maxStars - 1);
            }

            return finalStars;
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