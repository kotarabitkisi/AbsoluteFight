using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class VisualEffectManager : MonoBehaviour
{
    public Volume volume;
    public Vignette _vignette;
    public List<SlashClass> Slashes;
    public static VisualEffectManager Instance;
    public GameObject NyxSoulPooler;
    public List<ProjectileClass> Projectiles;




    public enum ProjectileId
    {
        Tiffany_WindBurst = 0,
        Tiffany_Veins = 1,
        Elara_RewardOrPunishment = 2,
        Arrow = 3,
        Samy_Shuriken = 4,
        Cyra_Reboot = 5,
        Melodi_Note_Attack = 6,
        Aurora_Punch = 7,
        Aurora_Ult = 8,
        Emilia_Heal = 9,
        Emilia_Push = 10,
        Emilia_RevivingAura = 11,
        Damian_Lazer = 12,
        Damian_Overheat = 13,
        Rebecca_SpinningSlash = 14,
        Slime_Blob = 15,
    }
    public enum SlashId
    {
        Nyx = 0,
        Betty = 1,
        Felix = 2,
        Syndra_Scythe = 3,
        Syndra_GetSoul = 4,
        Syndra_Execute = 5,
        Cyra_Slash = 6,
        Sakura = 7,
        Sakura_SilentDeath = 8,
        Damian_Slash = 9,
        Rebecca_SpearSlash = 10,
        Wolf_Slash=11,
        Slime_Slash = 12,
    }
    [Serializable]
    public class SlashClass
    {
        public string name;
        public SlashId id;
        public Color color;
        public GameObject Slash;
        public SlashMove[] Movements;
        [Serializable]
        public class SlashMove
        {
            public Vector2 position;
            public float duration;
        }
        public Sprite EffectIcon;
    }
    [Serializable]
    public class ProjectileClass
    {
        public string name;
        public ProjectileId id;
        public GameObject ObjectPooler;
        public bool RotateTowardsTarget;
    }
    void Awake()
    {
        if (volume.profile.TryGet(out Vignette vig_))
        {
            _vignette = vig_;
        }
        else
        {
            Debug.LogError("Vingette is not found");
        }
        Instance = this;
        for (int i = 0; i < NyxSoulPooler.transform.childCount; i++)
        {
            NyxSoulPooler.transform.GetChild(i).GetComponent<VisualEffect>().Stop();
        }
    }
    public void GetNyxSoul(Vector2 User, Vector2 target)
    {
        GameObject NyxSoul = NyxSoulPooler.transform.GetChild(0).gameObject;
        NyxSoul.transform.SetParent(null);
        NyxSoul.transform.position = target;
        NyxSoul.GetComponent<VisualEffect>().Play();
        NyxSoul.transform.DOMove(User, 1).OnComplete(() =>
        {
            NyxSoul.GetComponent<VisualEffect>().Stop();
            NyxSoul.transform.SetParent(NyxSoulPooler.transform);
        });
    }
    public void MakeVignetteColorEffect(Color color_, float value, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x, value, duration));
        sequence.Join(DOTween.To(() => _vignette.color.value, x => _vignette.color.value = x, color_, duration));
    }
    public IEnumerator MoveSlash(Vector2 target, SlashId WhichSlash)
    {
        SlashClass chosenSlash = null;
        foreach (SlashClass slash in Slashes)
        {
            if (slash.id == WhichSlash)
            {
                chosenSlash = slash;
            }
        }
        if (chosenSlash == null)
        {
            Debug.LogError("The Slash You Try To Find is null:" + WhichSlash);
        }

        GameObject Slash = chosenSlash.Slash;
        Slash.transform.position = target + chosenSlash.Movements[0].position;
        TrailRenderer Trail = Slash.GetComponent<TrailRenderer>();
        VisualEffect visualEffect = Slash.GetComponent<VisualEffect>();
        Trail.startColor = chosenSlash.color;
        Trail.endColor = chosenSlash.color;
        Trail.material.mainTexture = chosenSlash.EffectIcon.texture;
        Trail.enabled = true;
        Slash.transform.SetParent(null);
        visualEffect.enabled = true;
        visualEffect.Play();
        Sequence seq = DOTween.Sequence();
        for (int i = 1; i < chosenSlash.Movements.Length; i++)
        {
            seq.Append(Slash.transform.DOMoveX(target.x + chosenSlash.Movements[i].position.x, chosenSlash.Movements[i].duration).SetEase(Ease.OutCirc));
            seq.Join(Slash.transform.DOMoveY(target.y + chosenSlash.Movements[i].position.y, chosenSlash.Movements[i].duration).SetEase(Ease.Linear));
        }
        seq.AppendInterval(chosenSlash.Movements[^1].duration);
        seq.OnComplete(() =>
        {
            visualEffect.Stop();
            Trail.enabled = false;
        });
        yield return seq.WaitForCompletion();
    }

    public IEnumerator MoveProjectile(Vector2 User, Vector2 target, ProjectileId WhichProjectile, float duration)
    {
        ProjectileClass chosenProjectile = null;
        foreach (ProjectileClass proj in Projectiles)
        {
            if (proj.id == WhichProjectile)
            {
                chosenProjectile = proj;
            }
        }
        if (chosenProjectile == null)
        {
            Debug.LogError("The Projectile You Try To Find is null:" + WhichProjectile);
        }
        GameObject Projectile = SpawnProjectile(User, target, chosenProjectile);
        
        Projectile.transform.DOMove(target, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Projectile.GetComponent<VisualEffect>().Stop();
            Projectile.GetComponent<SpriteRenderer>().enabled = false;
            Projectile.transform.SetParent(chosenProjectile.ObjectPooler.transform);

        });
        yield return new WaitForSeconds(0.51f);
        yield break;
    }
    public GameObject SpawnProjectile(Vector2 User, Vector2 target, ProjectileClass chosenProjectile)
    {
        GameObject Projectile = chosenProjectile.ObjectPooler.transform.GetChild(0).gameObject;
        Projectile.GetComponent<SpriteRenderer>().enabled = true;
        Projectile.transform.SetParent(null);
        Projectile.transform.position = User;
        float angle = 0;
        
        if (chosenProjectile.RotateTowardsTarget)
        {
            Vector2 direction = target - User;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        Projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        Projectile.GetComponent<VisualEffect>().enabled = true;
        Projectile.GetComponent<VisualEffect>().Play();
        return Projectile;
    }
}
