using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Selector.MVVM_Selector;
using UnityEngine.UI;
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
             
        [Header("Timer Settings")]
        [SerializeField] private float challengeDuration = 60f;
        [SerializeField] private Slider challengeTimerSlider;
        [Header("Score and Star Settings")]
        [SerializeField] private StarInventory starInventory; // NOVO: Referência ao scriptable object
        [SerializeField] private int maxStars = 3;
        [SerializeField] private int initialScore = 100;
        [SerializeField] private int incorrectPenalty = 20; // Pontos perdidos por resposta errada
        [SerializeField] [Range(0, 1)] private float timeThreshold3Stars = 0.5f; // Acima de 50% do tempo restante

        private float currentScore;
        private int wrongAttempts;
        private float timeRemaining; 
        private bool challengeActive;

        private Action<bool> OnChallengeStarted;
        private Action<bool> OnChallengeEnded;

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

        private void Update()
        {
            if (challengeActive)
            {
                timeRemaining -= Time.deltaTime;
        
                // Atualiza o slider com o tempo restante
                if (challengeTimerSlider != null)
                {
                    challengeTimerSlider.value = timeRemaining / challengeDuration;
                }

                // Fim de jogo por tempo
                if (timeRemaining <= 0)
                {
                    challengeActive = false;
                    timeRemaining = 0;
                    OnChallengeEnded?.Invoke(false); // Falso para indicar falha
                    Debug.Log("Tempo esgotado! Tentar Novamente.");
                    // Você deve exibir a tela de "Tente Novamente" aqui.
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
            currentScore -= incorrectPenalty; // Penalidade por erro

            // Se a pontuação cair muito, ou se sobrar apenas 1 botão
            if (currentScore < 0) currentScore = 0; 

            Debug.Log($"Resposta Incorreta. Penalidade de {incorrectPenalty} pontos. Pontos atuais: {currentScore}");
        }

        private void RightAnswerSelected()
        {
            challengeActive = false;
    
            // 1. Calcular Estrelas
            int stars = CalculateStars();

            // 2. Salvar no Inventário
            if (starInventory != null)
            {
                starInventory.AddStars(stars);
            }
    
            OnChallengeEnded?.Invoke(true); // Verdadeiro para indicar sucesso
            Debug.Log($"Resposta Correta! Você ganhou {stars} estrela(s). Total acumulado: {starInventory.CurrentStars}");

            // Você pode chamar SetupNewChallenge() aqui para o próximo nível, se for o caso.
        }
        
        private int CalculateStars()
        {
            // CÁLCULO DAS ESTRELAS
    
            // 1. Estrela por Tempo
            float timeRatio = timeRemaining / challengeDuration;
            int starsFromTime = 0;

            // Se o jogador acertou acima do limite de 50% do tempo (timeThreshold3Stars)
            if (timeRatio > timeThreshold3Stars)
            {
                starsFromTime = maxStars; // 3 Estrelas
            }
            // Para simplificar, podemos definir 2 estrelas para 25% e 1 para menos, 
            // ou apenas usar a pontuação (Score). Usaremos o Score para a pontuação estratégica.

    
            // 2. Estrela por Pontuação (Score)
            int starsFromScore;
    
            // Se o score for 100% (ou seja, 0 erros e tempo rápido)
            if (currentScore >= initialScore)
            {
                starsFromScore = 3;
            }
            // Se houve 1 erro (100 - 20 = 80%) ou se o tempo passou um pouco
            else if (currentScore >= initialScore * 0.5f) 
            {
                starsFromScore = 2;
            }
            // Se houve 2 erros (100 - 40 = 60%) - AQUI ESTÁ A LÓGICA DO SEU REQUISITO
            // "se o jogador clicar em 2 respostas erradas e só sobrar a resposta correta ele só receberá 1 estrela"
            else if (wrongAttempts >= totalButtons - 1) 
            {
                starsFromScore = 1;
            }
            else
            {
                starsFromScore = 0;
            }
    
            // Vamos usar a estrela MÍNIMA garantida (1) e o máximo de 3.
            // Garante no mínimo 1 estrela se o jogador acertou, a menos que ele tenha falhado totalmente no score
            return Mathf.Max(1, starsFromScore); 
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