public class Minion
{
    public MinionSO MinionInfo { get; private set; }
    public Team Team { get; private set; }
    public bool IsTrainer { get; private set; }

    private float health;
    private float magic;

    public Minion(MinionSO minionInfo, Team team){
        MinionInfo = minionInfo;
        Team = team;
        IsTrainer = (MinionInfo.MinionId == MinionList.Boy) || (MinionInfo.MinionId == MinionList.Girl) ? true : false;
        health = minionInfo.HealthBase;
        magic = minionInfo.MagicBase;
    }

    public float MaxHealth(){
        return MinionInfo.HealthBase;
    }
    
    public float MaxMagic(){
        return MinionInfo.MagicBase;
    }
}
