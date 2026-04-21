using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
public class CharacterScript : MonoBehaviour
{
    public SpriteRenderer TeamColor;
    public GameObject ReviveEffect;
    public Image TurnSpeedBar;
    public Image HealthBar;
    public Image VirusBar;
    public CharacterScriptable CharScriptable;
    public CharacterMovement characterMovement;
    public float playTurn;
    public SpriteRenderer spriteRenderer;
    public CurrentStats stats;
    public CurrentStats defStatsAfterLevelThings;
    public Status status;
    public List<int> currentCooldown;
    [Serializable]
    public class CurrentStats
    {
        [Header("Main Stats")]
        public float health, maxHealth;
        [Range(0, 1)]
        public float defense;
        public int MovementSpeed;
        public float playSpeed;
        public int level;
        [Range(0, 1)]
        public float virusMultiplier;
        public int TeamType;
        public float healForEachTurn;
        public float evadeProbability;
        public float damage, virusDamage, penetration;
    }
    [Serializable]
    public class Status
    {
        public int Freeze;
        public int Struggle;
        public int Sakura_DodgeCount;
        public int Samy_ShurikenCount;
        public int Shade_TheShadowLairAura;
        public int moral;
        public int Nyx_SoulCount;
        public int Syndra_SoulCount;
        public int sleepTurn;
        public int CatSense;
        public int Cyra_AIThinking;
        public int Confuse;
        public List<PosionClass> poisons;
        public int bleed;

        public int RevivingAura;
        public int stunned;

        [Serializable]
        public class PosionClass
        {
            public int poisonTurn;
            public int poisonAmount;
        }
        public IEnumerator ReduceCooldownOfStatus(CharacterScript script)
        {
            VisualEffectManager.ProjectileClass chosenProjectile = null;
            foreach (VisualEffectManager.ProjectileClass proj in VisualEffectManager.Instance.Projectiles)
            {
                if (proj.id == VisualEffectManager.ProjectileId.Emilia_RevivingAura)
                {
                    chosenProjectile = proj;
                }
            }
            if (script.CharScriptable.charName == CharacterScriptable.CHARNAME.Emilia)
            {
                if (RevivingAura > 0)
                {
                    script.ReviveEffect.transform.SetParent(script.transform);
                    script.ReviveEffect.transform.localPosition = Vector3.zero;
                    script.ReviveEffect.GetComponent<VisualEffect>().enabled = true;
                    script.ReviveEffect.GetComponent<SpriteRenderer>().enabled = true;
                    RevivingAura--;
                }
                else
                {
                    script.ReviveEffect.transform.SetParent(null);
                    script.ReviveEffect.GetComponent<VisualEffect>().enabled = false;
                    script.ReviveEffect.GetComponent<VisualEffect>().Stop();
                    script.ReviveEffect.GetComponent<SpriteRenderer>().enabled = false;
                }
            }


            if (CatSense > 0)
            {
                CatSense--;
                script.stats.playSpeed = 1.2f * script.defStatsAfterLevelThings.playSpeed;
            }
            else
            {
                script.stats.playSpeed = script.defStatsAfterLevelThings.playSpeed;
            }
            for (int i = poisons.Count - 1; i >= 0; i--)
            {
                poisons[i].poisonTurn--;
                if (poisons[i].poisonTurn <= 0)
                {
                    poisons.RemoveAt(i);
                }
            }
            if (script.status.sleepTurn > 0)
            {
                Sequence seq = DOTween.Sequence();
                script.spriteRenderer.DOColor(Color.gray, 0.25f).OnComplete(() => script.spriteRenderer.DOColor(Color.white, 0.25f));
                yield return new WaitForSeconds(0.5f);
            }
            if (script.status.bleed > 0)
            {
                script.spriteRenderer.DOColor(Color.violetRed, 0.25f).OnComplete(() => script.spriteRenderer.DOColor(Color.white, 0.25f)).WaitForCompletion();
                script.StartCoroutine(script.TakeDamageWithoutAnimation(script.stats.maxHealth * 0.05f, 1));
            }
            Freeze--;
            if (Freeze <= 0)
            {
                script.stats.playSpeed = script.defStatsAfterLevelThings.playSpeed;
            }
            else
            {
                script.stats.playSpeed = 0.8f * script.defStatsAfterLevelThings.playSpeed;
            }
            script.status.Confuse--;


        }
    }
    public IEnumerator StartTurn()
    {
        characterMovement.GetCellPosition();
        stats.MovementSpeed = CharScriptable.MovementSpeed;
        if (status.Struggle > 0)
        {
            stats.MovementSpeed = 0;
        }
        yield return StartCoroutine(status.ReduceCooldownOfStatus(this));
        playTurn = 100;
        ReduceCooldown(1);
        if (status.sleepTurn > 0)
        {
            status.sleepTurn--;
            GameManager.instance.TakeTurn();
            yield break;
        }
        GameManager.instance.OpenUIOfThisCharacter(this);



        characterMovement.ControlAllReachableTiles();
    }
    public IEnumerator TakeVirus(CharacterScript Damager, float damage)
    {
        stats.virusMultiplier += damage * (1 - stats.defense);
        Sequence seq = DOTween.Sequence();
        seq.Append(spriteRenderer.DOColor(SkillManager.instance.selectedSkill.TransformedEnemy.VirusColor, 0.25f));
        seq.Append(spriteRenderer.DOColor(Color.white, 0.25f));
        VirusBar.DOFillAmount(stats.virusMultiplier, 0.25f);
        yield return new WaitForSeconds(0.8f);
        if (stats.virusMultiplier >= stats.health / stats.maxHealth)
        {
            yield return StartCoroutine(TurnToEnemy(Damager.stats.TeamType, SkillManager.instance.selectedSkill.TransformedEnemy));
        }
        yield return 0;
    }
    public IEnumerator TurnToEnemy(int Team, CharacterScriptable EnemyThatTurned)
    {
        spriteRenderer.material.SetColor("_VirusColor", EnemyThatTurned.VirusColor);

        Sequence infectionSeq = DOTween.Sequence();

        infectionSeq.Append(spriteRenderer.material.DOFloat(1f, "_Amount", 0.25f));

        infectionSeq.AppendCallback(() =>
        {
            stats.TeamType = Team;
            stats.damage = EnemyThatTurned.damage;
            stats.penetration = EnemyThatTurned.penetration;
            stats.defense = EnemyThatTurned.defense;
            stats.health = stats.health / stats.maxHealth * EnemyThatTurned.health;
            stats.maxHealth = EnemyThatTurned.health;
            stats.playSpeed = EnemyThatTurned.playSpeed;
            CharScriptable = EnemyThatTurned;
            playTurn = 200;
            stats.virusMultiplier = 0;
            stats.virusDamage = EnemyThatTurned.damage;
            spriteRenderer.sprite = CharScriptable.charIcon;
        });
        infectionSeq.Join(VirusBar.DOFillAmount(0, 0.25f));
        infectionSeq.Join(HealthBar.DOFillAmount(1, 0.25f));
        infectionSeq.Append(spriteRenderer.material.DOFloat(0f, "_Amount", 0.25f));
        infectionSeq.Append(transform.DOScale(Vector3.one * 1.1f, 0.25f));
        infectionSeq.Append(transform.DOScale(Vector3.one, 0.25f));
        yield return new WaitForSecondsRealtime(1.25f);

        yield return 0;



    }
    public IEnumerator TakeDamage(CharacterScript Damager, float damage, float penetration)
    {
        if (Damager.status.Confuse > 0 && UnityEngine.Random.Range(0f, 1f) <= 0.20f)
        {
            StartCoroutine(Damager.TakeDamage(Damager, damage, penetration));
            yield break;

        }
        else if (status.Sakura_DodgeCount > 0)
        {
            status.Sakura_DodgeCount--;
            yield return transform.DOShakePosition(0.4f, Vector2.right * 0.05f, vibrato: 20).WaitForCompletion();
            yield break;
        }
        Vector3 firstPos = transform.position;
        Sequence seq = DOTween.Sequence();
        if (Damager.CharScriptable.charName == CharacterScriptable.CHARNAME.Nyx)
        {
            status.Nyx_SoulCount++;
        }
        if (UnityEngine.Random.Range(0f, 1f) <= stats.evadeProbability)
        {
            seq.Append(transform.DOMove(Vector3.right * 0.2f + firstPos, 0.25f));
            seq.Append(transform.DOMove(firstPos, 0.25f));
            yield return seq.WaitForCompletion();
            yield break;
        }


        yield return StartCoroutine(TakeDamageWithoutAnimation(damage, penetration));
        seq.Append(transform.DOMove((Damager.transform.position - firstPos).normalized * -0.2f + firstPos, 0.25f));
        seq.Append(transform.DOMove(firstPos, 0.25f));
        yield return seq.WaitForCompletion();
    }
    public IEnumerator Heal(float amount)
    {
        stats.health += amount;
        if (stats.health >= stats.maxHealth)
        {
            stats.health = stats.maxHealth;
        }
        Sequence seq = DOTween.Sequence();
        seq.Append(spriteRenderer.DOColor(Color.green, 0.25f));
        seq.Append(spriteRenderer.DOColor(Color.white, 0.25f));
        seq.Append(HealthBar.DOFillAmount(stats.health / stats.maxHealth, 0.25f)).WaitForCompletion();
        yield return null;
    }
    public IEnumerator TakeDamage(Color color, float damage)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(spriteRenderer.DOColor(color, 0.25f));
        seq.Append(spriteRenderer.DOColor(Color.white, 0.25f)).OnComplete(() => StartCoroutine(TakeDamageWithoutAnimation(damage, 1)));
        yield return seq.WaitForCompletion();
    }
    public IEnumerator TakeDamageWithoutAnimation(float damage, float penetration)
    {
        stats.health -= damage * (1 - Mathf.Clamp(stats.defense - penetration, 0, 1));
        print(damage * (1 - Mathf.Clamp(0, 1, stats.defense - penetration)));
        HealthBar.DOFillAmount(stats.health / stats.maxHealth, 0.25f).WaitForCompletion();
        if (stats.health >= stats.maxHealth)
        {
            stats.health = stats.maxHealth;
        }
        yield return StartCoroutine(IsDead());
    }
    public IEnumerator Push(CharacterScript Damager)
    {
        Debug.Log($"[Push] Fonksiyon başladı. Damager: {Damager.name}");

        Vector2Int currentPos = characterMovement.gridpos;
        Vector2Int attackerPos = Damager.characterMovement.gridpos;

        Vector2Int pushDir = currentPos - attackerPos;
        Vector2Int targetGridPos = currentPos + pushDir / (int)pushDir.magnitude;

        Debug.Log($"[Push] Mevcut Pos: {currentPos}, Saldırgan Pos: {attackerPos}");
        Debug.Log($"[Push] Hesaplanan Yön: {pushDir}, Hedef Grid: {targetGridPos}");

        bool cellExists = GridMapManager.instance.gridData.TryGetValue(targetGridPos, out var targetCell);

        if (cellExists)
        {
            bool isFull = targetCell.unitOnTop != null;
            Debug.Log($"[Push] Hedef Hücre Bulundu. Üzerinde biri var mı?: {isFull}");

            if (!isFull)
            {
                Debug.Log("[Push] Koşullar sağlandı, hareket başlıyor...");
                GridMapManager.instance.gridData[currentPos].unitOnTop = null;
                Vector3 worldTarget = targetCell.tileScript.transform.position;
                yield return transform.DOMove(worldTarget, 0.25f).SetEase(Ease.OutQuad).WaitForCompletion();

                characterMovement.gridpos = targetGridPos;
                targetCell.unitOnTop = this;
                Debug.Log("[Push] Hareket tamamlandı ve veriler güncellendi.");
            }
            else
            {
                Debug.LogWarning($"[Push] HATA: Hedef hücre dolu! ({targetCell.unitOnTop.name})");
                yield return transform.DOShakePosition(0.2f, 0.15f).WaitForCompletion();
            }
        }
        else
        {
            Debug.LogError($"[Push] HATA: Hedef hücre ({targetGridPos}) gridData içinde bulunamadı (Harita dışı?)");
            yield return transform.DOShakePosition(0.2f, 0.15f).WaitForCompletion();
        }
    }
    public IEnumerator IsDead()
    {
        if (stats.health > 0)
        {
            yield break;
        }
        if (GameManager.instance.allCharacterScripts.Contains(this))
        {
            GameManager.instance.allCharacterScripts.Remove(this);
        }

        GridMapManager.instance.gridData[characterMovement.gridpos].unitOnTop = null;

        yield return transform.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
        {
            Destroy(gameObject, 0.05f);
        }).WaitForCompletion();
    }

    public void ReduceCooldown(int amount)
    {
        for (int i = 0; i < CharScriptable.Skills.Length; i++)
        {
            currentCooldown[i] -= amount;
        }
    }
}
