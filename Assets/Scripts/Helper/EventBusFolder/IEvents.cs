namespace Helper.EventBusFolder
{

    public class GameState //guardando referencias
    {
        public static FarmManager CurrentFarm { get; set; }
    }
    
    public interface IEvents { }

    public class OnChallengeCompleted : IEvents
    {
        public int StarsGained { get; }

        public OnChallengeCompleted(int starsGained)
        {
            StarsGained = starsGained;
        }
    }

    public class UpdateCurrentFarmEvent : IEvents
    {
        public FarmManager Farm { get; }

        public UpdateCurrentFarmEvent(FarmManager farm)
        {
            Farm = farm;
        }
    }

    public class OnNextLevelEvent : IEvents
    {
        public int Level { get; }
        
        public OnNextLevelEvent(int level)
        {
            Level = level;
        }
    }

    #region SecondLevel
    public class OnSecondLevelInitiateEvent : IEvents
    {
        public OnSecondLevelInitiateEvent() { }
    }

    public class OnSequenceInitialized : IEvents
    {
        public SequenceManager sequenceManager;

        public OnSequenceInitialized(SequenceManager seqManager)
        {
            sequenceManager = seqManager;
        }
    }

    public class OnResetLevel02Event : IEvents
    {
        public OnResetLevel02Event() { }
    }
    
    #endregion
}