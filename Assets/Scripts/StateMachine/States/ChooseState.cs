using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChooseState : BaseState
{
    private readonly KeyCode ACTION1_KEY = KeyCode.Q;
    private readonly KeyCode ACTION2_KEY = KeyCode.W;
    private TurnStateMachine TSM;
    private Vector2Int currentHover;
    private List<Vector2Int> availableMovement, availableAction1, availableAction2, availableSummon;
    private MinionUnit selectedMinion;
    private ChooseOptions chooseOption;
    private Action action1, action2;
    private bool isSummonPanelEnabled;
    private enum ChooseOptions
    {
        None,
        Move,
        Action1,
        Action2,
        Summon
    }
    public ChooseState(TurnStateMachine stateMachine) : base("Choose", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        selectedMinion = TSM.GetSelectedMinion();
        chooseOption = ChooseOptions.Move;
        GetAvailableTiles();
        Gameboard.Instance.ChangeTilesLayers(availableMovement, TileLayer.Highlight);
        TSM.DeselectTargetPosition();
        isSummonPanelEnabled = false;
        UIManager.Instance.closeSummonEvent.AddListener(CloseSummonPanel);
    }

    private void GetAvailableTiles()
    {
        availableMovement = selectedMinion.minion.GetAvailableMoves(ref TSM.GetMinionUnitsArray(), selectedMinion.MinionIndex);
        action1 = selectedMinion.minion.action1;
        action2 = selectedMinion.minion.action2;
        availableAction1 = action1 != null ? action1.GetAvailableAttackTiles(ref TSM.GetMinionUnitsArray(), selectedMinion.MinionIndex, TSM.GetEnemyTeam()) : null;
        availableAction2 = action2 != null ? action2.GetAvailableAttackTiles(ref TSM.GetMinionUnitsArray(), selectedMinion.MinionIndex, TSM.GetEnemyTeam()) : null;
        availableSummon = selectedMinion.IsTrainer ? selectedMinion.minion.GetAvailableSummons(ref TSM.GetMinionUnitsArray(), selectedMinion.MinionIndex) : null;
    }

    public override void Update()
    {
        base.Update();
        currentHover = Gameboard.Instance.GetCurrentHover();
        
        if (Input.GetMouseButtonDown(1)){ //Right clik -> Go back: Sel. < Mov. < Att.
            Debug.Log(chooseOption);
            isSummonPanelEnabled = false;
            UIManager.Instance.DisableSummonMinionUI();
            if (chooseOption == ChooseOptions.Move)
            {
                stateMachine.ChangeState(TSM.selectionState);
                return;
            }
            else
            {
                chooseOption = ChooseOptions.Move;
                UpdateTileVisuals();
                return;
            }
        }
        if (isSummonPanelEnabled)
            return;
        
        if (Input.GetMouseButtonDown(0))
            { //Left click
                if (currentHover == -Vector2Int.one)
                { //Invalid Tile: Select another Minion
                    stateMachine.ChangeState(TSM.selectionState);
                    UIManager.Instance.DisableSummonMinionUI();
                    return;
                }
                switch (chooseOption)
                {
                    case ChooseOptions.None:
                        return;
                    case ChooseOptions.Move:
                        if (IsValidMovement(currentHover))
                        {
                            TSM.SetTargetPosition(currentHover);
                            stateMachine.ChangeState(TSM.moveState);
                            return;
                        }
                        return;
                    case ChooseOptions.Action1:
                        if (IsValidAttack(currentHover))
                        {
                            TSM.SetTargetPosition(currentHover);
                            TSM.SelectAction(selectedMinion.minion.action1);
                            stateMachine.ChangeState(TSM.attackState);
                        }
                        return;
                    case ChooseOptions.Action2:
                        if (IsValidAttack(currentHover))
                        {
                            TSM.SetTargetPosition(currentHover);
                            TSM.SelectAction(selectedMinion.minion.action2);
                            stateMachine.ChangeState(TSM.attackState);
                        }
                        return;
                    case ChooseOptions.Summon:
                        throw new System.Exception("NOT IMPLEMENTED");

                    default:
                        throw new System.Exception("Error: Wrong Choose Options.");
                }
                // else if(minionUnits[currentHover.x, currentHover.y].Team == currentPlayerTurn){
                //     SwitchSelectMinion(currentHover);
                // }
            }
        if(Input.GetKeyDown(ACTION1_KEY)){
            if (IsSummonPossible(1))
            {
                SetUpSummonPanel();
                return;
            }
            if (!IsActionElegible(ChooseOptions.Action1))
            {
                chooseOption = ChooseOptions.Move;
                UpdateTileVisuals();
                return;
            } 
            chooseOption = ChooseOptions.Action1;
            UpdateTileVisuals();
            Debug.Log(chooseOption);
            return;
        }
        if(Input.GetKeyDown(ACTION2_KEY))
        {
            if (IsSummonPossible(2))
            {
                SetUpSummonPanel();
                return;
            }
            if (!IsActionElegible(ChooseOptions.Action2))
            {
                chooseOption = ChooseOptions.Move;
                UpdateTileVisuals();
                return;
            } 
            chooseOption = ChooseOptions.Action2;
            UpdateTileVisuals();
            Debug.Log(chooseOption);
            UIManager.Instance.DisableSummonMinionUI();
            return;
        }

        // if (Input.GetKeyDown(SUMMON_KEY))
        // {
        //     if (!IsSummonPossible())
        //     {
        //         UIManager.Instance.DisableSummonMinionUI();
        //         chooseOption = ChooseOptions.Move;
        //         UpdateTileVisuals();
        //         return;
        //     }
        //     UIManager.Instance.SetupSummonMinionUI(TSM.GetMinionUnitList(TSM.GetCurrentPlayerTurn()));
        //     chooseOption = ChooseOptions.Summon;
        //     UpdateTileVisuals();
        //     Debug.Log(chooseOption);
        // } 
    }

    private bool IsValidMovement(Vector2Int index)
    {
        if (TSM.GetMinionUnit(currentHover) != null)
        {
            selectedMinion.QueueMessage("There is already someone there!");
            return false;
        }
        if (availableMovement.Contains(index))
        {
            return true;
        }
        selectedMinion.QueueMessage("I can't go there!");
        return false;
    }

    private bool IsValidAttack(Vector2Int index){
        if(TSM.GetMinionUnit(currentHover) == null) return false;
        if(TSM.GetMinionUnit(currentHover).Team == TSM.GetCurrentPlayerTurn()) return false;

        List<Vector2Int> availableTiles = chooseOption == ChooseOptions.Action1? availableAction1 : availableAction2;
        if(availableTiles.Contains(index)){
            return true;
        }
        return false;
    }

    private bool IsActionElegible(ChooseOptions posibleAction)
    {

        Action action = posibleAction == ChooseOptions.Action1? selectedMinion.minion.action1 : selectedMinion.minion.action2;
        if(action == null){
            selectedMinion.QueueMessage("I don't have actions!");
            return false;
        }
        List<Vector2Int> availableTiles = posibleAction == ChooseOptions.Action1? availableAction1 : availableAction2;

        if (!selectedMinion.HasEnoughMagic(action)) return false;
        if (availableTiles.Count < 1)
        {
            selectedMinion.QueueMessage("Enemy out of range!");
            return false;
        }

        return true;
    }
    
    private bool IsSummonPossible(int actionNumber)
    {
        if (!selectedMinion.IsTrainer)
        {
            selectedMinion.QueueMessage("I can't summon!");
            return false;
        }
        if (availableSummon.Count < 1)
        {
            selectedMinion.QueueMessage("No enough tiles!");
            return false;
        }
        if (actionNumber == 1 && action1.ActionInfo.Name != "Summon")
        {
            selectedMinion.QueueMessage("I can't summon 1!");
            return false;
        }
        if (actionNumber == 2 && action2.ActionInfo.Name != "Summon")
        {
            selectedMinion.QueueMessage("I can't summon 2!");
            return false;
        }      
        
        return true;
    }

    private void SetUpSummonPanel()
    {
        isSummonPanelEnabled = true;
        UIManager.Instance.SetupSummonMinionUI(TSM.GetMinionUnitList(TSM.GetCurrentPlayerTurn()));
        chooseOption = ChooseOptions.Summon;
        UpdateTileVisuals();
        Debug.Log(chooseOption);
        return;
    }

    private void CloseSummonPanel()
    {
        isSummonPanelEnabled = false;
        UIManager.Instance.DisableSummonMinionUI();
        chooseOption = ChooseOptions.Move;
        UpdateTileVisuals();
    }

    private void UpdateTileVisuals()
    {
        Gameboard.Instance.SetAllTilesToDefaultLayer();
        switch (chooseOption)
        {
            case ChooseOptions.None:
                return;
            case ChooseOptions.Move:
                if (availableMovement.Count < 1) throw new System.Exception("Empty availableMovement");
                Gameboard.Instance.ChangeTilesLayers(availableMovement, TileLayer.Highlight);
                return;
            case ChooseOptions.Action1:
                if (availableAction1.Count < 1) throw new System.Exception("Empty availableAction1");
                Gameboard.Instance.ChangeTilesLayers(availableAction1, TileLayer.Danger);
                return;
            case ChooseOptions.Action2:
                if (availableAction2.Count < 1) throw new System.Exception("Empty availableAction2");
                Gameboard.Instance.ChangeTilesLayers(availableAction2, TileLayer.Danger);
                return;
            case ChooseOptions.Summon:
                if (availableSummon.Count < 1) throw new System.Exception("Empty availableAction2");
                Gameboard.Instance.ChangeTilesLayers(availableSummon, TileLayer.Danger);
                return;

            default:
                throw new System.Exception("Error: Wrong Choose Options.");
        }
    }

    public override void Exit()
    {
        base.Exit();
        Gameboard.Instance.SetAllTilesToDefaultLayer();
        UIManager.Instance.DisableSummonMinionUI();
    }
}
