using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
public class SkillManager : MonoBehaviour
{

    public SkillScriptable selectedSkill;
    public CharacterScript chosenCharToUseSkill;
    public Dictionary<Vector2Int, GridMapManager.Node> chosengrids = new();
    public static SkillManager instance;
    void Awake()
    {
        instance = this;
    }
    public void UseSkillBtnPressedVoid()
    {
        StartCoroutine(UseskillBtnPressed());
    }
    public IEnumerator UseskillBtnPressed()
    {
        GridMapManager.instance.ClearHighlights();
        List<Vector2Int> chosenTiles = new();
        foreach (Vector2Int chosenGrid in chosengrids.Keys)
        {
            chosenTiles.Add(chosenGrid);
        }
        if (selectedSkill != null)
        {
            yield return StartCoroutine(UseSkill(selectedSkill.skillId, chosenTiles, chosenCharToUseSkill));
        }
        else
        {
            GameManager.instance.TakeTurn();
        }

    }
    public void SkillCancel(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;
        chosengrids.Clear();
        selectedSkill = null;
        chosenCharToUseSkill.characterMovement.ControlAllReachableTiles();
    }
    public IEnumerator UseSkill(SkillScriptable.SkillIds id, List<Vector2Int> chosenTiles, CharacterScript WhoAreUsingIt)
    {
        if (selectedSkill.justTapToUse)
        {
            switch (id)
            {
                case SkillScriptable.SkillIds.Default_AllAttack:
                    foreach (Vector2Int gridPos_ in GridMapManager.instance.GetNeighbors(chosenCharToUseSkill.characterMovement.gridpos))
                    {
                        if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target)
                        {
                            if (target.stats.TeamType != chosenCharToUseSkill.stats.TeamType)
                            {
                                switch (chosenCharToUseSkill.CharScriptable.charName)
                                {
                                    case CharacterScriptable.CHARNAME.Nyx:
                                        yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (target.transform.position, VisualEffectManager.SlashId.Nyx)); break;
                                    case CharacterScriptable.CHARNAME.Sakura:
                                        yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (target.transform.position, VisualEffectManager.SlashId.Sakura)); break;

                                }
                                yield return StartCoroutine(target.TakeDamage(chosenCharToUseSkill, chosenCharToUseSkill.stats.damage, chosenCharToUseSkill.stats.penetration));
                            }
                        }

                    }
                    break;
                case SkillScriptable.SkillIds.Elara_FightingForHim:
                    yield return StartCoroutine(SpecialSkillLogic.Elara_FightingForHim());
                    break;
                case SkillScriptable.SkillIds.Elara_TheSinnerMustDie:
                    yield return StartCoroutine(SpecialSkillLogic.Elara_TheSinnerMustDie());
                    break;
                case SkillScriptable.SkillIds.Tiffany_ThePowerOfNature:
                    yield return StartCoroutine(SpecialSkillLogic.Tiffany_ThePowerOfNature());
                    break;
                case SkillScriptable.SkillIds.Shade_TheShadowLairAura:
                    yield return StartCoroutine(SpecialSkillLogic.Shade_TheShadowLairAura());
                    break;
                case SkillScriptable.SkillIds.Cyra_AIThinking:
                    yield return StartCoroutine(SpecialSkillLogic.Cyra_AIThinking());
                    break;
                case SkillScriptable.SkillIds.Betty_MakeAllCharacterCatSense:
                    yield return StartCoroutine(SpecialSkillLogic.Betty_MakeAllCharacterCatSense());
                    break;
                case SkillScriptable.SkillIds.Felix_GetSpeed:
                    yield return StartCoroutine(SpecialSkillLogic.Felix_GetSpeed());
                    break;
                case SkillScriptable.SkillIds.Nyx_AbsorbAllOfPain:
                    yield return StartCoroutine(SpecialSkillLogic.Nyx_AbsorbAllOfPain(WhoAreUsingIt));
                    break;
                case SkillScriptable.SkillIds.Syndra_Scythe:
                    yield return StartCoroutine(SpecialSkillLogic.Syndra_Scythe());
                    break;
                case SkillScriptable.SkillIds.Melodi_Trying_Opera:
                    yield return StartCoroutine(SpecialSkillLogic.Melodi_Trying_Opera());
                    break;
                case SkillScriptable.SkillIds.Sakura_PetalDance:
                    yield return StartCoroutine(SpecialSkillLogic.Sakura_PetalDance());
                    break;
                case SkillScriptable.SkillIds.Rebecca_BleedingDance:
                    yield return StartCoroutine(SpecialSkillLogic.Rebecca_BleedingDance(chosenCharToUseSkill));
                    break;
                case SkillScriptable.SkillIds.Emilia_RevivingAura:
                    yield return StartCoroutine(SpecialSkillLogic.Emilia_RevivingAura(chosenCharToUseSkill));
                    break;
                case SkillScriptable.SkillIds.SlimeGirl_BlowUp:
                    yield return StartCoroutine(SpecialSkillLogic.SlimeGirl_BlowUp(WhoAreUsingIt));
                    break;
            }
        }
        else
        {
            foreach (Vector2Int Charpos in chosenTiles)
            {
                CharacterScript target = GridMapManager.instance.gridData[Charpos].unitOnTop;
                switch (id)
                {
                    case SkillScriptable.SkillIds.Default_Melee:
                        switch (WhoAreUsingIt.CharScriptable.charName)
                        {
                            case CharacterScriptable.CHARNAME.Nyx:
                                yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                              (target.transform.position, VisualEffectManager.SlashId.Nyx)); break;
                            case CharacterScriptable.CHARNAME.Sakura:
                                yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                              (target.transform.position, VisualEffectManager.SlashId.Sakura)); break;
                            case CharacterScriptable.CHARNAME.DefaultEnemy_Wolf:
                                yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                              (target.transform.position, VisualEffectManager.SlashId.Wolf_Slash)); break;

                        }
                        yield return StartCoroutine(target.TakeDamage(WhoAreUsingIt, WhoAreUsingIt.stats.damage, WhoAreUsingIt.stats.penetration));

                        break;
                    case SkillScriptable.SkillIds.Default_Ranged:
                        yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(WhoAreUsingIt.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Arrow, 0.5f));
                        yield return StartCoroutine(target.TakeDamage(WhoAreUsingIt, WhoAreUsingIt.stats.damage, WhoAreUsingIt.stats.penetration));

                        break;
                    case SkillScriptable.SkillIds.Betty_Scratch:
                        yield return StartCoroutine(SpecialSkillLogic.Betty_Scratch(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Betty_FluffyPaw:
                        yield return StartCoroutine(SpecialSkillLogic.Betty_FluffyPaw(target));
                        break;
                    case SkillScriptable.SkillIds.Betty_Meow:
                        yield return StartCoroutine(SpecialSkillLogic.Betty_Meow(target));
                        break;
                    case SkillScriptable.SkillIds.Elara_RewardOrPunishment:
                        yield return StartCoroutine(SpecialSkillLogic.Elara_RewardOrPunishment(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Elara_Moral:
                        yield return StartCoroutine(SpecialSkillLogic.Elara_Moral(target));
                        break;
                    case SkillScriptable.SkillIds.Tiffany_WindThrow:
                        yield return StartCoroutine(SpecialSkillLogic.Tiffany_WindThrow(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Tiffany_TheWelcomingOfForest:
                        yield return StartCoroutine(SpecialSkillLogic.Tiffany_TheWelcomingOfForest(WhoAreUsingIt, target));
                        break;


                    case SkillScriptable.SkillIds.Shade_Push:
                        yield return StartCoroutine(SpecialSkillLogic.Shade_Push(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Shade_SwitchWithTeammate:
                        yield return StartCoroutine(SpecialSkillLogic.Shade_SwitchWithTeammate(WhoAreUsingIt, target));
                        break;


                    case SkillScriptable.SkillIds.Samy_Shuriken:
                        yield return StartCoroutine(SpecialSkillLogic.Samy_Shuriken(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Samy_DirtClone:
                        yield return StartCoroutine(SpecialSkillLogic.Samy_DirtClone((Vector3Int)Charpos, WhoAreUsingIt));
                        break;
                    case SkillScriptable.SkillIds.Samy_Revenge:
                        yield return StartCoroutine(SpecialSkillLogic.Samy_Revenge(WhoAreUsingIt, target));
                        break;

                    case SkillScriptable.SkillIds.Felix_Cut:
                        yield return StartCoroutine(SpecialSkillLogic.Felix_Cut(WhoAreUsingIt, target));
                        break;

                    case SkillScriptable.SkillIds.Felix_LookMyKatana:
                        yield return StartCoroutine(SpecialSkillLogic.Felix_LookMyKatana(WhoAreUsingIt, target));
                        break;

                    case SkillScriptable.SkillIds.Cyra_CpuCut:
                        yield return StartCoroutine(SpecialSkillLogic.Cyra_CpuCut(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Cyra_Reboot:
                        yield return StartCoroutine(SpecialSkillLogic.Cyra_Reboot(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Syndra_GetSoul:
                        yield return StartCoroutine(SpecialSkillLogic.Syndra_GetSoul(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Syndra_Execute:
                        yield return StartCoroutine(SpecialSkillLogic.Syndra_Execute(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Melodi_Note_Attack:
                        yield return StartCoroutine(SpecialSkillLogic.Melodi_NoteAttack(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Melodi_Accent_Incident:
                        yield return StartCoroutine(SpecialSkillLogic.Melodi_AccentIncident(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Sakura_BlinkStrike:
                        yield return StartCoroutine(SpecialSkillLogic.Sakura_BlinkStrike(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Sakura_SilentDeath:
                        yield return StartCoroutine(SpecialSkillLogic.Sakura_SilentDeath(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Aurora_Punch:
                        yield return StartCoroutine(SpecialSkillLogic.Aurora_Punch(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Aurora_FrostBreath:
                        yield return StartCoroutine(SpecialSkillLogic.Aurora_FrostBreath(target));
                        break;
                    case SkillScriptable.SkillIds.Aurora_TheIceDragonsWraith:
                        yield return StartCoroutine(SpecialSkillLogic.Aurora_TheIceDragonsWraith(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Damian_HeatSword:
                        yield return StartCoroutine(SpecialSkillLogic.Damian_HeatSword(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Damian_HeatLazer:
                        yield return StartCoroutine(SpecialSkillLogic.Damian_HeatLazer(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Damian_OverheatBlow:
                        yield return StartCoroutine(SpecialSkillLogic.Damian_OverheatBlow(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Rebecca_SpearSlash:
                        yield return StartCoroutine(SpecialSkillLogic.Rebecca_SpearSlash(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Rebecca_SpinningSlash:
                        yield return StartCoroutine(SpecialSkillLogic.Rebecca_SpinningSlash(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Johnson_BigPunch:
                        yield return StartCoroutine(SpecialSkillLogic.Johnson_BigPunch(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Johnson_HitWithSledge:
                        yield return StartCoroutine(SpecialSkillLogic.Johnson_HitWithSledge(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Johnson_Balmond:
                        yield return StartCoroutine(SpecialSkillLogic.Johnson_Balmond(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Emilia_GoAway:
                        yield return StartCoroutine(SpecialSkillLogic.Emilia_GoAway(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Emilia_Healing:
                        yield return StartCoroutine(SpecialSkillLogic.Emilia_Healing(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Slime_Blob:
                        yield return StartCoroutine(SpecialSkillLogic.Slime_Blob(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.Slime_Assimilate:
                        yield return StartCoroutine(SpecialSkillLogic.Slime_Assimilate(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.SlimeGirl_TentacleHit:
                        yield return StartCoroutine(SpecialSkillLogic.SlimeGirl_TentacleHit(WhoAreUsingIt, target));
                        break;
                    case SkillScriptable.SkillIds.SlimeGirl_SlimeGlue:
                        yield return StartCoroutine(SpecialSkillLogic.SlimeGirl_SlimeGlue(WhoAreUsingIt, target));
                        break;





                    default:
                        Debug.LogWarning("Tanımlanmamış Skill ID: " + id);
                        break;
                }
            }
        }

        chosengrids.Clear();
        StartCooldown();
        if (selectedSkill.isTurnFinishes)
        {
            GameManager.instance.TakeTurn();
        }
        else
        {
            GameManager.instance.CloseUI();
            GameManager.instance.OpenUIOfThisCharacter(chosenCharToUseSkill);
        }
        yield return null;
    }
    public void SelectSkillVoid(SkillScriptable selectedSkill_)
    {
        chosengrids.Clear();
        GridMapManager.instance.ClearHighlights();
        StartCoroutine(SelectSkill(selectedSkill_));
    }
    public IEnumerator SelectSkill(SkillScriptable selectedSkill_)
    {

        selectedSkill = selectedSkill_;
        if (selectedSkill_.justTapToUse)
        {

            yield return StartCoroutine(UseSkill(selectedSkill_.skillId,
            null,
            GameManager.instance.ChosenChar));
            selectedSkill = null;
        }
        else
        {

            GridMapManager.instance.GetUsableTiles(chosenCharToUseSkill.characterMovement.gridpos);
        }
    }
    public void SelectUsableChar(Vector2Int selectedchar)
    {
        print("Usable Char Selected");
        if (GridMapManager.instance.attackHighlightedGrids.ContainsKey(selectedchar))
        {
            if (ItIsUsable(selectedchar) && chosengrids.Count != selectedSkill.useCount)
            {
                chosengrids.Add(selectedchar, GridMapManager.instance.attackHighlightedGrids[selectedchar]);
                GridMapManager.instance.tilemap.SetTileFlags((Vector3Int)selectedchar, TileFlags.None);
                Color color = Color.white;
                if (GridMapManager.instance.gridData[selectedchar].unitOnTop == null)
                {
                    if (instance.selectedSkill.AvailableTarget.HasFlag(SkillScriptable.TargetType.Enemy))
                    {
                        color = Color.lightBlue;
                    }
                }
                else if (GridMapManager.instance.gridData[selectedchar].unitOnTop.stats.TeamType != chosenCharToUseSkill.stats.TeamType
                && instance.selectedSkill.AvailableTarget.HasFlag(SkillScriptable.TargetType.Enemy))
                {
                    color = Color.darkRed;
                }
                else if (GridMapManager.instance.gridData[selectedchar].unitOnTop.stats.TeamType == chosenCharToUseSkill.stats.TeamType
                && instance.selectedSkill.AvailableTarget.HasFlag(SkillScriptable.TargetType.Ally))
                {
                    color = Color.lightGreen;
                }
                GridMapManager.instance.tilemap.SetColor((Vector3Int)selectedchar, color);
                if (selectedSkill.useCount == 1)
                {
                    StartCoroutine(UseskillBtnPressed());
                }
            }
        }
    }
    public bool ItIsUsable(Vector2Int selectedGrid)
    {
        var targetGrid = GridMapManager.instance.attackHighlightedGrids[selectedGrid];
        SkillScriptable.TargetType currentTileType = SkillScriptable.TargetType.NONE;
        if (targetGrid.unitOnTop != null)
        {
            bool isEnemy = targetGrid.unitOnTop.stats.TeamType != chosenCharToUseSkill.stats.TeamType;
            currentTileType = isEnemy ? SkillScriptable.TargetType.Enemy : SkillScriptable.TargetType.Ally;
        }
        else if (!targetGrid.tileScript.tileScr.isWall)
        {
            currentTileType = SkillScriptable.TargetType.EmptyTile;
        }
        bool isTargetValid = selectedSkill.AvailableTarget.HasFlag(currentTileType);
        bool alreadyChosen = chosengrids.ContainsKey(selectedGrid);

        return isTargetValid && !alreadyChosen;

    }
    public void StartCooldown()
    {
        for (int i = 0; i < chosenCharToUseSkill.CharScriptable.Skills.Length; i++)
        {
            if (chosenCharToUseSkill.CharScriptable.Skills[i] == selectedSkill)
            {
                chosenCharToUseSkill.currentCooldown[i] = chosenCharToUseSkill.CharScriptable.Skills[i].cooldown;
            }
        }
    }
    public static class SpecialSkillLogic
    {
        #region Betty
        public static IEnumerator Betty_FluffyPaw(CharacterScript target)
        {
            yield return instance.StartCoroutine(target.TakeVirus(instance.chosenCharToUseSkill, 0.05f));
            if (Random.Range(0f, 1f) <= 0.75f)
            {
                target.spriteRenderer.DOColor(Color.gray, 0.1f);
                target.status.sleepTurn = 1;
                yield return new WaitForSeconds(0.1f);
                target.spriteRenderer.DOColor(Color.white, 0.1f);
            }
            yield return 0;
        }
        public static IEnumerator Betty_Scratch(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (target.transform.position, VisualEffectManager.SlashId.Betty));
            float virusAmount = target.status.sleepTurn > 0 ? 0.1f : 0.05f;
            yield return instance.StartCoroutine(target.TakeDamage(user, user.stats.damage * 0.5f, user.stats.penetration));
            yield return instance.StartCoroutine(target.TakeVirus(instance.chosenCharToUseSkill, virusAmount));
        }
        public static IEnumerator Betty_MakeAllCharacterCatSense()
        {
            List<Vector2Int> neighbourtiles = GridMapManager.instance.GetNeighbors(instance.chosenCharToUseSkill.characterMovement.gridpos);
            foreach (Vector2Int neighbour in neighbourtiles)
            {
                CharacterScript target = GridMapManager.instance.gridData[neighbour].unitOnTop;
                if (target != null &&
                target.stats.TeamType == instance.chosenCharToUseSkill.stats.TeamType)
                {
                    Sequence seq = DOTween.Sequence();
                    seq.Append(target.spriteRenderer.DOColor(Color.gray, 0.2f));
                    seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
                    yield return seq.WaitForCompletion();
                    target.status.CatSense = 2;
                }
            }

        }
        public static IEnumerator Betty_Meow(CharacterScript target)
        {
            float virusAmount = target.status.sleepTurn > 0 ? 0.5f : 0.2f;
            yield return instance.StartCoroutine(target.TakeVirus(instance.chosenCharToUseSkill, virusAmount));
        }
        #endregion
        #region Nyx
        public static IEnumerator Nyx_AbsorbAllOfPain(CharacterScript damager)
        {
            List<CharacterScript> allchars = GameManager.instance.allCharacterScripts;
            for (int i = 0; i < allchars.Count; i++)
            {
                if (allchars[i].status.Nyx_SoulCount > 0)
                {
                    print(i);
                    Sequence seq = DOTween.Sequence();
                    CharacterScript target = allchars[i];
                    target.StartCoroutine(target.TakeDamageWithoutAnimation(damager.stats.damage * instance.selectedSkill.skillDamageMultiplier * target.status.Nyx_SoulCount, 1));
                    seq.Append(target.spriteRenderer.DOColor(Color.purple, 0.2f));
                    target.StartCoroutine(target.TakeDamageWithoutAnimation(-damager.stats.damage * instance.selectedSkill.skillDamageMultiplier * target.status.Nyx_SoulCount / 2, 1));
                    seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
                    VisualEffectManager.Instance.GetNyxSoul(damager.transform.position, target.transform.position);
                    target.status.Nyx_SoulCount = 0;
                }
            }
            yield return new WaitForSeconds(1.5f);

            damager.spriteRenderer.DOColor(Color.purple, 0.2f).OnComplete(() => { damager.spriteRenderer.DOColor(Color.white, 0.2f); });
        }
        #endregion

        public static IEnumerator Elara_RewardOrPunishment(CharacterScript user, CharacterScript target)
        {
            Sequence seq = DOTween.Sequence();
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Elara_RewardOrPunishment, 0.5f));
            if (target.stats.TeamType == user.stats.TeamType)
            {
                target.StartCoroutine(target.TakeDamageWithoutAnimation(-target.stats.maxHealth * 0.05f, 1));
                seq.Join(target.spriteRenderer.DOColor(Color.yellow, 0.2f));
                seq.Join(target.spriteRenderer.DOColor(Color.white, 0.2f));
            }
            else
            {
                target.StartCoroutine(target.TakeDamageWithoutAnimation(target.stats.maxHealth * 0.05f, 1));
                seq.Append(target.spriteRenderer.DOColor(Color.black, 0.2f));
                seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
            }
            yield return new WaitForSeconds(0.4f);
        }
        public static IEnumerator Elara_Moral(CharacterScript target)
        {
            target.status.moral = 2;
            Sequence seq = DOTween.Sequence();
            seq.Append(target.spriteRenderer.DOColor(Color.yellow, 0.2f));
            seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
            yield return new WaitForSeconds(0.4f);
        }
        public static IEnumerator Elara_FightingForHim()
        {
            List<Vector2Int> neighbourtiles = GridMapManager.instance.GetNeighbors(instance.chosenCharToUseSkill.characterMovement.gridpos);
            foreach (Vector2Int neighbour in neighbourtiles)
            {
                CharacterScript char_ = GridMapManager.instance.gridData[neighbour].unitOnTop;
                if (char_ != null &&
                char_.stats.TeamType == instance.chosenCharToUseSkill.stats.TeamType)
                {
                    yield return char_.StartCoroutine(char_.Heal(char_.stats.maxHealth * 0.2f));
                }
            }
            yield return null;
        }
        public static IEnumerator Elara_TheSinnerMustDie()
        {
            yield return null;
        }
        public static IEnumerator Tiffany_WindThrow(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Tiffany_WindBurst, 0.5f));
            yield return target.StartCoroutine(target.TakeDamage(user, instance.selectedSkill.skillDamageBase + user.stats.damage * instance.selectedSkill.skillDamageMultiplier, user.stats.penetration));
            yield return target.StartCoroutine(target.Push(user));
        }
        public static IEnumerator Tiffany_TheWelcomingOfForest(CharacterScript user, CharacterScript target)
        {
            target.playTurn += 100;

            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Tiffany_Veins, 0.5f));
            Sequence seq = DOTween.Sequence();
            seq.Append(target.spriteRenderer.DOColor(Color.darkGreen, 0.2f));
            seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));

            seq.WaitForCompletion();
        }
        public static IEnumerator Tiffany_ThePowerOfNature()
        {
            List<Vector2Int> neighbourtiles = GridMapManager.instance.GetNeighbors(instance.chosenCharToUseSkill.characterMovement.gridpos);
            foreach (Vector2Int neighbour in neighbourtiles)
            {
                CharacterScript Target = GridMapManager.instance.gridData[neighbour].unitOnTop;
                if (Target != null &&
                Target.stats.TeamType == instance.chosenCharToUseSkill.stats.TeamType)
                {
                    Target.playTurn += 50;
                    Sequence seq = DOTween.Sequence();
                    yield return Target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(instance.chosenCharToUseSkill.transform.position, Target.transform.position, VisualEffectManager.ProjectileId.Tiffany_Veins, 0.5f));
                    seq.Append(Target.spriteRenderer.DOColor(Color.darkGreen, 0.2f));
                    seq.Append(Target.spriteRenderer.DOColor(Color.white, 0.2f));
                    seq.Join(Target.TurnSpeedBar.DOFillAmount(1, 0.2f)).WaitForCompletion();
                }
            }
            yield return null;
        }
        public static IEnumerator Shade_Push(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(target.Push(user));
        }
        public static IEnumerator Shade_SwitchWithTeammate(CharacterScript user, CharacterScript target)
        {
            Vector3 userPos = user.transform.position;
            Vector3 targetPos = target.transform.position;

            user.transform.DOMove(targetPos, 0.25f);
            target.transform.DOMove(userPos, 0.25f);
            yield return new WaitForSeconds(0.3f);
            user.characterMovement.GetCellPosition();
            target.characterMovement.GetCellPosition();

        }
        public static IEnumerator Shade_TheShadowLairAura()
        {
            CharacterScript user = instance.chosenCharToUseSkill;
            List<Vector2Int> neighbourtiles = GridMapManager.instance.GetNeighbors(user.characterMovement.gridpos);
            foreach (Vector2Int tilePos in neighbourtiles)
            {
                CharacterScript target = GridMapManager.instance.gridData[tilePos].unitOnTop;
                if (target == null) continue;
                Sequence seq = DOTween.Sequence();
                seq.Append(target.spriteRenderer.DOColor(Color.gray1, 0.2f));
                seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
                yield return new WaitForSeconds(0.4f);
                target.status.Shade_TheShadowLairAura = 1;
            }
            user.transform.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
            {
                Destroy(user.gameObject);
                GameManager.instance.InitializeCharacterList();
            });
        }

        public static IEnumerator Samy_Shuriken(CharacterScript whoAreUsingIt, CharacterScript charstat)
        {
            yield return charstat.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(whoAreUsingIt.transform.position, charstat.transform.position, VisualEffectManager.ProjectileId.Samy_Shuriken, 0.5f));
            yield return charstat.StartCoroutine(charstat.TakeDamage(whoAreUsingIt, whoAreUsingIt.stats.damage, 1));
        }

        public static IEnumerator Samy_DirtClone(Vector3 pos, CharacterScript whoAreUsingIt)
        {
            //Indevelopment
            yield return null;
        }

        public static IEnumerator Samy_Revenge(CharacterScript whoAreUsingIt, CharacterScript target)
        {
            float damage = whoAreUsingIt.stats.damage * (1 + 0.05f * whoAreUsingIt.status.Samy_ShurikenCount);
            Vector3 firstpos = whoAreUsingIt.transform.position;
            whoAreUsingIt.transform.DOMove(target.transform.position, 0.25f);
            yield return new WaitForSeconds(0.25f);
            yield return target.StartCoroutine(target.TakeDamage(whoAreUsingIt,
            whoAreUsingIt.stats.damage * (1 + 0.05f * whoAreUsingIt.status.Samy_ShurikenCount), 1));
            if (damage < target.stats.health)
            {
                whoAreUsingIt.transform.DOMove(firstpos, 0.25f);
                yield return new WaitForSeconds(0.25f);
            }
            whoAreUsingIt.characterMovement.GetCellPosition();
            yield return null;

        }

        internal static IEnumerator Felix_Cut(CharacterScript whoAreUsingIt, CharacterScript charstat)
        {
            yield return charstat.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (charstat.transform.position, VisualEffectManager.SlashId.Nyx));
            yield return charstat.StartCoroutine(charstat.TakeDamage(whoAreUsingIt, whoAreUsingIt.stats.damage, 1));
        }

        public static IEnumerator Felix_GetSpeed()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(instance.chosenCharToUseSkill.spriteRenderer.DOColor(Color.lightBlue, 0.2f));
            seq.Join(instance.chosenCharToUseSkill.transform.DOShakePosition(0.4f, Vector2.right * 0.05f, vibrato: 20));
            seq.Append(instance.chosenCharToUseSkill.spriteRenderer.DOColor(Color.white, 0.2f));

            yield return new WaitForSeconds(0.4f);
            instance.chosenCharToUseSkill.stats.playSpeed *= 1.1f;
        }

        public static IEnumerator Felix_LookMyKatana(CharacterScript whoAreUsingIt, CharacterScript target)
        {
            float damage = whoAreUsingIt.stats.damage * (1 + 0.1f * whoAreUsingIt.stats.playSpeed);
            float health = target.stats.health;
            Vector3 firstpos = whoAreUsingIt.transform.position;
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.darkRed, 0.5f, 0.5f);
            yield return whoAreUsingIt.transform.DOMove(target.transform.position, 0.25f).WaitForCompletion();
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (target.transform.position, VisualEffectManager.SlashId.Felix));

            yield return target.StartCoroutine(target.TakeDamage(whoAreUsingIt,
            damage, 1));
            if (health > damage)
            {
                whoAreUsingIt.transform.DOMove(firstpos, 0.25f).WaitForCompletion();
            }
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0, 0.5f);
            whoAreUsingIt.characterMovement.GetCellPosition();
            yield return null;
        }

        internal static IEnumerator Cyra_CpuCut(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (target.transform.position, VisualEffectManager.SlashId.Cyra_Slash));
            yield return instance.StartCoroutine(target.TakeDamage(user, user.stats.damage, user.stats.penetration));
            yield return instance.StartCoroutine(target.TakeVirus(instance.chosenCharToUseSkill, 0.05f));
        }

        internal static IEnumerator Cyra_AIThinking()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(instance.chosenCharToUseSkill.spriteRenderer.DOColor(Color.lightSkyBlue, 0.2f));
            seq.Append(instance.chosenCharToUseSkill.spriteRenderer.DOColor(Color.white, 0.2f));
            yield return new WaitForSeconds(0.4f);
            instance.chosenCharToUseSkill.status.Cyra_AIThinking = 3;
            List<Vector2Int> neighbours = GridMapManager.instance.GetNeighbors(instance.chosenCharToUseSkill.characterMovement.gridpos);
            foreach (Vector2Int character in neighbours)
            {
                CharacterScript char_ = GridMapManager.instance.gridData[character].unitOnTop;
                if (char_ != null)
                {
                    if (char_.stats.TeamType == instance.chosenCharToUseSkill.stats.TeamType)
                    {
                        seq.Append(char_.spriteRenderer.DOColor(Color.lightSkyBlue, 0.2f));
                        seq.Append(char_.spriteRenderer.DOColor(Color.white, 0.2f));
                        yield return new WaitForSeconds(0.4f);
                        char_.status.Cyra_AIThinking = 3;
                    }
                }
            }
        }

        internal static IEnumerator Cyra_Reboot(CharacterScript user, CharacterScript target)
        {
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.skyBlue, 0.5f, 0.5f);
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Cyra_Reboot, 0.5f));
            yield return instance.StartCoroutine(target.TakeVirus(instance.chosenCharToUseSkill, instance.selectedSkill.skillVirusPercent));
        }

        internal static IEnumerator Syndra_Scythe()
        {
            CharacterScript SyndraScript = instance.chosenCharToUseSkill;
            yield return SyndraScript.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                         (SyndraScript.transform.position, VisualEffectManager.SlashId.Syndra_Scythe));
            int a = 0;
            List<Coroutine> coroutineList = new();
            foreach (Vector2Int gridPos_ in GridMapManager.instance.GetNeighbors(SyndraScript.characterMovement.gridpos))
            {
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target)
                {
                    if (target.stats.TeamType != SyndraScript.stats.TeamType)
                    {
                        a++;
                        coroutineList.Add(instance.StartCoroutine(target.TakeDamage(SyndraScript, SyndraScript.stats.damage * (1 + 0.05f * SyndraScript.status.Syndra_SoulCount), SyndraScript.stats.penetration)));
                    }
                }
            }
            SyndraScript.status.Syndra_SoulCount += a;
            foreach (Coroutine cor in coroutineList)
            {
                yield return cor;
            }
        }

        internal static IEnumerator Syndra_GetSoul(CharacterScript whoAreUsingIt, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (target.transform.position, VisualEffectManager.SlashId.Syndra_GetSoul));
            Sequence seq = DOTween.Sequence();
            seq.Append(instance.chosenCharToUseSkill.spriteRenderer.DOColor(Color.purple, 0.2f));
            seq.Append(instance.chosenCharToUseSkill.spriteRenderer.DOColor(Color.white, 0.2f));
            instance.StartCoroutine(target.TakeDamage(whoAreUsingIt, target.stats.health * 0.005f * whoAreUsingIt.status.Syndra_SoulCount, 1));
            whoAreUsingIt.status.Syndra_SoulCount += 5;
        }

        internal static IEnumerator Syndra_Execute(CharacterScript whoAreUsingIt, CharacterScript target)
        {
            float damage = whoAreUsingIt.stats.damage * (1 + 0.05f * whoAreUsingIt.status.Syndra_SoulCount);
            float health = target.stats.health;
            Vector3 firstpos = whoAreUsingIt.transform.position;
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.purple, 0.5f, 0.5f);
            whoAreUsingIt.transform.DOMove(target.transform.position, 0.25f).WaitForCompletion();
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (target.transform.position, VisualEffectManager.SlashId.Syndra_Execute));

            yield return target.StartCoroutine(target.TakeDamage(whoAreUsingIt,
            damage, 1));
            if (health > damage)
            {
                whoAreUsingIt.transform.DOMove(firstpos, 0.25f).WaitForCompletion();
            }
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0, 0.5f);
            whoAreUsingIt.characterMovement.GetCellPosition();
            yield return null;
        }

        internal static IEnumerator Melodi_NoteAttack(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Melodi_Note_Attack, 0.5f));
            yield return target.StartCoroutine(target.TakeDamage(user, user.stats.damage * instance.selectedSkill.skillDamageMultiplier, user.stats.penetration));

        }

        internal static IEnumerator Melodi_AccentIncident(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Melodi_Note_Attack, 0.5f));
            target.status.Confuse = 2;
        }

        internal static IEnumerator Melodi_Trying_Opera()
        {
            CharacterScript MelodiScript = instance.chosenCharToUseSkill;
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.red, 0.75f, 0.75f);
            foreach (Vector2Int gridPos_ in GridMapManager.instance.GetNeighbors(MelodiScript.characterMovement.gridpos))
            {
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target)
                {
                    if (target.stats.TeamType != MelodiScript.stats.TeamType)
                    {
                        Sequence seq = DOTween.Sequence();
                        seq.Append(target.spriteRenderer.DOColor(Color.red, 0.2f));
                        seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
                        yield return seq.WaitForCompletion();
                        target.status.bleed = 2;
                    }
                }
            }
            Camera.main.transform.DOShakePosition(0.5f, Vector2.right * 0.1f);
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0, 0.75f);
            yield break;


        }

        internal static IEnumerator Sakura_BlinkStrike(CharacterScript whoAreUsingIt, CharacterScript target)
        {
            List<Vector2Int> gridPos = new();
            foreach (Vector2Int gridPos_ in GridMapManager.instance.GetNeighbors(target.characterMovement.gridpos))
            {
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop == null)
                {
                    gridPos.Add(gridPos_);
                }

            }
            Vector2Int chosenGrid = gridPos[Random.Range(0, gridPos.Count)];
            Vector3 position = GridMapManager.instance.tilemap.GetCellCenterWorld(new Vector3Int(chosenGrid.x, chosenGrid.y, 0));

            Sequence seq = DOTween.Sequence();
            yield return
            whoAreUsingIt.transform.DOMove(position, 0.75f).WaitForCompletion();
            seq.Append(target.spriteRenderer.DOColor(Color.red, 0.2f));
            seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
            whoAreUsingIt.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                         (target.transform.position, VisualEffectManager.SlashId.Sakura));
            yield return target.StartCoroutine(target.TakeDamage(whoAreUsingIt,
instance.selectedSkill.skillDamageMultiplier * whoAreUsingIt.stats.damage, whoAreUsingIt.stats.penetration));
            yield return seq.WaitForCompletion();

            whoAreUsingIt.characterMovement.GetCellPosition();
            target.status.bleed = 2;
        }
        internal static IEnumerator Sakura_PetalDance()
        {
            CharacterScript Sakura = instance.chosenCharToUseSkill;
            Sequence seq = DOTween.Sequence();
            seq.Append(Sakura.spriteRenderer.DOColor(Color.pink, 0.2f));
            seq.Append(Sakura.spriteRenderer.DOColor(Color.white, 0.2f));
            Sakura.transform.DOShakePosition(0.5f, Vector2.right * 0.1f);
            yield return seq.WaitForCompletion();
            Sakura.status.Sakura_DodgeCount = 2;
        }
        internal static IEnumerator Sakura_SilentDeath(CharacterScript whoAreUsingIt, CharacterScript target)
        {
            float damage = whoAreUsingIt.stats.damage + 0.3f * (target.stats.maxHealth - target.stats.health);
            float health = target.stats.health;
            Vector3 firstpos = whoAreUsingIt.transform.position;
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.hotPink, 0.5f, 0.5f);
            whoAreUsingIt.transform.DOMove(target.transform.position, 0.25f).WaitForCompletion();
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                      (target.transform.position, VisualEffectManager.SlashId.Sakura_SilentDeath));

            yield return target.StartCoroutine(target.TakeDamage(whoAreUsingIt,
            damage, 1));
            if (health > damage)
            {
                yield return whoAreUsingIt.transform.DOMove(firstpos, 0.25f).WaitForCompletion();
            }
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0, 0.5f);
            whoAreUsingIt.characterMovement.GetCellPosition();
            yield return null;
        }

        internal static IEnumerator Aurora_TheIceDragonsWraith(CharacterScript user, CharacterScript target)
        {
            CharacterScript AuroraScript = instance.chosenCharToUseSkill;
            yield return AuroraScript.StartCoroutine(VisualEffectManager.Instance.MoveProjectile
                         (target.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Aurora_Ult, 1f));
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.blueViolet, 0.75f, 2);
            Sequence seq = DOTween.Sequence();
            List<Vector2Int> gridPoses = GridMapManager.instance.GetNeighbors(target.characterMovement.gridpos);
            foreach (Vector2Int gridPos_ in gridPoses)
            {
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target_)
                {
                    if (target_.stats.TeamType != AuroraScript.stats.TeamType)
                    {
                        target.StartCoroutine(target_.TakeDamageWithoutAnimation(AuroraScript.stats.damage * (1 + 0.05f * AuroraScript.status.Syndra_SoulCount), AuroraScript.stats.penetration));
                        target_.stats.playSpeed--;
                        target_.status.Freeze = 3;

                        seq.Join(target_.spriteRenderer.DOColor(Color.blueViolet, 0.2f)).OnComplete(() => seq.Join(target_.spriteRenderer.DOColor(Color.white, 0.2f)));

                    }
                }
            }
            seq.AppendInterval(0.25f);
            foreach (Vector2Int gridPos_ in gridPoses)
            {
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target_)
                {
                    seq.Join(target_.spriteRenderer.DOColor(Color.white, 0.2f));
                }
            }

            if (Random.Range(0f, 1f) <= 0.75f)
            {
                user.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
                {
                    Destroy(user.gameObject);
                }).WaitForCompletion();
            }
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0.75f, 2);


        }

        internal static IEnumerator Aurora_FrostBreath(CharacterScript target)
        {
            target.stats.playSpeed--;
            target.status.Freeze = 2;
            Sequence seq = DOTween.Sequence();
            seq.Append(target.spriteRenderer.DOColor(Color.blueViolet, 0.2f));
            seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
            yield return seq.WaitForCompletion();
        }

        internal static IEnumerator Aurora_Punch(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Aurora_Punch, 0.5f));
            yield return target.StartCoroutine(target.TakeDamage(user, user.stats.damage, user.stats.penetration));
            if (Random.Range(0, 1) <= 0.5f)
            {
                target.stats.playSpeed--;
                target.status.Freeze = 2;
                Sequence seq = DOTween.Sequence();
                seq.Append(target.spriteRenderer.DOColor(Color.blueViolet, 0.2f));
                seq.Append(target.spriteRenderer.DOColor(Color.white, 0.2f));
                yield return seq.WaitForCompletion();
            }
        }

        public static IEnumerator Damian_HeatSword(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                                 (target.transform.position, VisualEffectManager.SlashId.Damian_Slash));
            yield return target.StartCoroutine(target.TakeDamage(user, user.stats.damage, user.stats.penetration));
        }

        public static IEnumerator Damian_HeatLazer(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile
                                                             (user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Damian_Lazer, 0.5f));
            yield return target.StartCoroutine(target.TakeDamage(user, user.stats.damage, user.stats.penetration));
        }

        public static IEnumerator Damian_OverheatBlow(CharacterScript user, CharacterScript target)
        {
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.red, 0.75f, 2);
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile
                                                                         (user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Damian_Overheat, 0.5f));
            yield return target.StartCoroutine(target.TakeDamage(user, user.stats.damage, user.stats.penetration));
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0.75f, 2);
        }

        public static IEnumerator Rebecca_SpearSlash(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                                            (target.transform.position, VisualEffectManager.SlashId.Rebecca_SpearSlash));
            yield return target.StartCoroutine(target.TakeDamage(user, user.stats.damage, user.stats.penetration));
            if (Random.Range(0f, 1f) <= 0.25f)
            {
                yield return target.spriteRenderer.DOColor(Color.red, 0.2f).OnComplete(() => target.spriteRenderer.DOColor(Color.white, 0.2f)).WaitForCompletion();
                target.status.bleed = 1;
            }
        }

        public static IEnumerator Rebecca_SpinningSlash(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile
                                                (user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Rebecca_SpinningSlash, 0.5f));
            List<Vector2Int> gridPoses = GridMapManager.instance.GetNeighbors(target.characterMovement.gridpos);
            Sequence seq = DOTween.Sequence();

            foreach (Vector2Int gridPos_ in gridPoses)
            {
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target_)
                {
                    if (target_.stats.TeamType != user.stats.TeamType)
                    {
                        Vector3 targetfirstpos = target_.transform.position;
                        target.StartCoroutine(target_.TakeDamageWithoutAnimation(user.stats.damage, 1));
                        if (Random.Range(0f, 1f) <= 0.25f)
                        {
                            target_.spriteRenderer.DOColor(Color.red, 0.2f).OnComplete(() => target_.spriteRenderer.DOColor(Color.white, 0.2f));
                            target_.status.bleed = 1;
                        }
                    }
                }
            }
        }

        public static IEnumerator Rebecca_BleedingDance(CharacterScript user)
        {
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.red, 0.75f, 2);
            Vector3 firstpos = user.transform.position;
            List<CharacterScript> Chars = GameManager.instance.allCharacterScripts;
            foreach (CharacterScript target_ in Chars)
            {
                if (target_.stats.TeamType != user.stats.TeamType && target_.status.bleed > 0)
                {
                    List<Vector2Int> gridPos = new();
                    foreach (Vector2Int gridPos_ in GridMapManager.instance.GetNeighbors(target_.characterMovement.gridpos))
                    {
                        if (GridMapManager.instance.gridData[gridPos_].unitOnTop == null)
                        {
                            gridPos.Add(gridPos_);
                        }
                    }
                    if (gridPos.Count == 0)
                    {
                        gridPos.Add(target_.characterMovement.gridpos);
                    }
                    Vector2Int chosenGrid = gridPos[Random.Range(0, gridPos.Count)];
                    Vector3 position = GridMapManager.instance.tilemap.GetCellCenterWorld(new Vector3Int(chosenGrid.x, chosenGrid.y, 0));

                    Sequence seq = DOTween.Sequence();
                    yield return user.transform.DOMove(position, 0.2f).WaitForCompletion();
                    user.StartCoroutine(VisualEffectManager.Instance.MoveSlash
                                 (target_.transform.position, VisualEffectManager.SlashId.Rebecca_SpearSlash));
                    yield return target_.StartCoroutine(target_.TakeDamage(user,
                    instance.selectedSkill.skillDamageMultiplier * user.stats.damage, user.stats.penetration));
                    yield return seq.WaitForCompletion();
                }
            }
            user.transform.DOMove(firstpos, 0.5f).OnComplete(() => user.characterMovement.GetCellPosition());
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0f, 2);
        }

        public static IEnumerator Johnson_BigPunch(CharacterScript whoAreUsingIt, CharacterScript target)
        {
            yield return target.StartCoroutine(target.TakeDamage(whoAreUsingIt, whoAreUsingIt.stats.damage, 0));
            if (Random.Range(0f, 1f) <= 0.25f)
            {
                target.spriteRenderer.DOColor(Color.yellow, 0.2f).OnComplete(() => target.spriteRenderer.DOColor(Color.white, 0.2f));
                target.status.stunned = 1;
            }
        }

        public static IEnumerator Johnson_HitWithSledge(CharacterScript whoAreUsingIt, CharacterScript target)
        {
            yield return target.StartCoroutine(target.TakeDamage(whoAreUsingIt, whoAreUsingIt.stats.damage, 0));
            if (Random.Range(0f, 1f) <= 0.25f)
            {
                target.spriteRenderer.DOColor(Color.yellow, 0.2f).OnComplete(() => target.spriteRenderer.DOColor(Color.white, 0.2f));
                target.status.stunned = 1;
            }
        }

        public static IEnumerator Johnson_Balmond(CharacterScript user, CharacterScript target)
        {

            Vector3 firstpos = user.transform.position;
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.darkRed, 0.5f, 0.5f);
            yield return user.transform.DOMove(target.transform.position, 0.75f).WaitForCompletion();
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0.75f, 2);
            List<Vector2Int> gridPoses = GridMapManager.instance.GetNeighbors(target.characterMovement.gridpos);

            foreach (Vector2Int gridPos_ in gridPoses)
            {
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target_)
                {
                    if (target_.stats.TeamType != user.stats.TeamType)
                    {
                        Sequence seq = DOTween.Sequence();
                        Vector3 targetfirstpos = target_.transform.position;
                        seq.Join(target_.transform.DOMoveY(targetfirstpos.y + 0.2f, 0.1f).SetEase(Ease.OutQuad));
                        seq.Append(target_.transform.DOMoveY(targetfirstpos.y, 0.1f).SetEase(Ease.InQuad));
                        if (Random.Range(0f, 1f) <= 0.25f)
                        {
                            target_.spriteRenderer.DOColor(Color.yellow, 0.2f).OnComplete(() => target_.spriteRenderer.DOColor(Color.white, 0.2f));
                            target_.status.stunned = 1;
                        }
                    }
                }
            }
            target.status.stunned = 2;
            target.spriteRenderer.DOColor(Color.yellow, 0.2f).OnComplete(() => target.spriteRenderer.DOColor(Color.white, 0.2f));
            yield return user.transform.DOMove(firstpos, 0.75f).WaitForCompletion();
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0f, 2);
        }

        public static IEnumerator Emilia_GoAway(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Emilia_Push, 0.5f));
            yield return target.StartCoroutine(target.TakeDamage(user, instance.selectedSkill.skillDamageBase + user.stats.damage * instance.selectedSkill.skillDamageMultiplier, user.stats.penetration));
            yield return target.StartCoroutine(target.Push(user));
        }

        public static IEnumerator Emilia_Healing(CharacterScript user, CharacterScript target)
        {
            Sequence seq = DOTween.Sequence();
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile(user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Emilia_Heal, 0.5f));
            target.StartCoroutine(target.TakeDamageWithoutAnimation(-target.stats.maxHealth * 0.05f, 1));
            seq.Join(target.spriteRenderer.DOColor(Color.yellow, 0.2f));
            seq.Join(target.spriteRenderer.DOColor(Color.white, 0.2f));
            List<Vector2Int> gridPoses = GridMapManager.instance.GetNeighbors(target.characterMovement.gridpos);
            foreach (Vector2Int gridPos_ in gridPoses)
            {
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target_)
                {
                    if (target_.stats.TeamType != user.stats.TeamType)
                    {
                        target.StartCoroutine(target.TakeDamageWithoutAnimation(-target.stats.maxHealth * 0.05f, 1));
                        seq.Join(target_.spriteRenderer.DOColor(Color.green, 0.2f)).OnComplete(() => seq.Join(target_.spriteRenderer.DOColor(Color.white, 0.2f)));
                    }
                }
            }
        }

        public static IEnumerator Emilia_RevivingAura(CharacterScript user)
        {
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.hotPink, 0.75f, 1);
            yield return new WaitForSeconds(1);
            user.status.RevivingAura = 2;
            user.ReviveEffect.transform.SetParent(user.transform);
            user.ReviveEffect.transform.localPosition = Vector3.zero;
            user.ReviveEffect.GetComponent<SpriteRenderer>().enabled = true;
            user.ReviveEffect.GetComponent<VisualEffect>().enabled = true;
            user.ReviveEffect.GetComponent<VisualEffect>().Play();
            VisualEffectManager.Instance.MakeVignetteColorEffect(Color.white, 0f, 1);

        }

        internal static IEnumerator Slime_Blob(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile
            (user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Slime_Blob, 0.75f));
            yield return target.StartCoroutine(target.TakeVirus(user, 0.05f));
        }

        internal static IEnumerator Slime_Assimilate(CharacterScript user, CharacterScript target)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(user.transform.DOMove(target.transform.position, 0.5f));
            seq.Append(user.spriteRenderer.DOColor(new Color(0, 0, 0, 0), 1));
            yield return seq.WaitForCompletion();
            yield return target.StartCoroutine(target.TakeVirus(user, 0.25f));
            user.stats.health = 0;
            yield return target.StartCoroutine(user.IsDead());
        }

        internal static IEnumerator SlimeGirl_TentacleHit(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveSlash
            (target.transform.position, VisualEffectManager.SlashId.Slime_Slash));
            yield return target.StartCoroutine(target.TakeDamage(user, user.stats.damage, user.stats.penetration));
            yield return target.StartCoroutine(target.TakeVirus(user, 0.05f));
        }

        internal static IEnumerator SlimeGirl_SlimeGlue(CharacterScript user, CharacterScript target)
        {
            yield return target.StartCoroutine(VisualEffectManager.Instance.MoveProjectile
            (user.transform.position, target.transform.position, VisualEffectManager.ProjectileId.Slime_Blob, 0.75f));
            yield return target.StartCoroutine(target.TakeDamage(user, user.stats.damage, user.stats.penetration));
            yield return target.StartCoroutine(target.TakeVirus(user, 0.10f));
            target.status.Struggle = 2;
        }

        internal static IEnumerator SlimeGirl_BlowUp(CharacterScript user)
        {
            List<Vector2Int> gridPoses = GridMapManager.instance.GetNeighbors(user.characterMovement.gridpos);

            foreach (Vector2Int gridPos_ in gridPoses)
            {
                yield return user.StartCoroutine(
                    VisualEffectManager.Instance.MoveProjectile
                    (user.transform.position,
                    GridMapManager.instance.tilemap.CellToWorld(new Vector3Int(gridPos_.x, gridPos_.y, 0)
                                    ),

                VisualEffectManager.ProjectileId.Slime_Blob, 0.75f));
                if (GridMapManager.instance.gridData[gridPos_].unitOnTop == null)
                {
                    user.StartCoroutine(GameManager.instance.SpawnCharacter
                    (
                        new PlayerDataScriptable.CharacterData
                        {
                            CharacterDefData = GameManager.instance.allCharacterScriptables[24],
                            Health = 1,
                            Level = user.stats.level,
                            Exp = 0,
                            ReqExp = 0,
                        }, instance.chosenCharToUseSkill.stats.TeamType, gridPos_));
                }
                else if (GridMapManager.instance.gridData[gridPos_].unitOnTop is CharacterScript target_)
                {
                    if (target_.stats.TeamType != user.stats.TeamType)
                    {
                        target_.StartCoroutine(target_.TakeVirus(user, 0.25f));
                    }
                }
            }
            yield return new WaitForSeconds(2);
            user.stats.health = 0;
            user.StartCoroutine(user.IsDead());
        }
    }
}
