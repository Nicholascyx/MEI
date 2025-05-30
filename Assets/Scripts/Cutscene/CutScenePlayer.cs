using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CutScenePlayer : Interactable
{
    [SerializeField] private GDTFadeEffect _fadeEffect;

    protected void FreezePlayer(PlayerController player, float positionX)
    {
        Vector3 playerPos = player.transform.position;
        player.transform.position = new Vector3(positionX, playerPos.y, playerPos.z);
        player.StopPlayerMovement();
    }

    protected void SetFadeParametersWithPingPong()
    {
        _fadeEffect.firstColor = Color.clear;
        _fadeEffect.lastColor = Color.black;
        _fadeEffect.pingPong = true;
        _fadeEffect.disableWhenFinish = true;
    }

    protected void SetFadeParametersWithoutPingPong()
    {
        _fadeEffect.firstColor = Color.clear;
        _fadeEffect.lastColor = Color.black;
        _fadeEffect.pingPong = false;
        _fadeEffect.disableWhenFinish = true;
    }

    public void ActivateFadeEffect()
    {
        _fadeEffect.gameObject.SetActive(true);
    }

    protected void ResetToPlayer()
    {
        PlayerController.Instance.ResetCamera();
        PlayerController.Instance.ResumePlayerMovement();
    }

    public IEnumerator ActivateCutSceneFlow(PlayableDirector director, PlayableAsset asset, MovePosition.Position freezePos, float customFreezePosX, bool canPlayerMove)
    {
        float fadeDuration = _fadeEffect.CalculateFadeDuration();
        float cutsceneDurationOffset = 1.0f;

        // Fade into the cutscene.
        SetFadeParametersWithPingPong();
        ActivateFadeEffect();
        yield return new WaitForSeconds(fadeDuration);

        if (!canPlayerMove)
        {
            // Freeze the player somewhere.
            Vector3 freezePosition = MovePosition.GetMovePosX(freezePos, new Vector3(customFreezePosX, 0.0f, 0.0f));
            FreezePlayer(PlayerController.Instance, freezePosition.x);
        }

        

        
        director.Play(asset);
        // Wait for the cutscene to play out.
        yield return new WaitForSeconds((float) asset.duration - cutsceneDurationOffset);

        // Fade out of the cutscene.
        SetFadeParametersWithoutPingPong();
        ActivateFadeEffect();
        fadeDuration = _fadeEffect.CalculateFadeDuration();
        yield return new WaitForSeconds(fadeDuration);

        director.Stop();
    }

}
