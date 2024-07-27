public class Minion
{
    public MinionSO MinionInfo { get; private set; }

    private float health;
    private float magic;

    public Minion(MinionSO minionInfo){
        MinionInfo = minionInfo;
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
