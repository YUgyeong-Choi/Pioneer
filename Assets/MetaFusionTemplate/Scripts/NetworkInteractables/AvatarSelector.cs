using Fusion;
using Oculus.Interaction;
using UnityEngine;
using static Chiligames.MetaFusionTemplate.AvatarSpawner;

namespace Chiligames.MetaFusionTemplate
{
    public class AvatarSelector : NetworkBehaviour
    {
        private FusionMetaAvatar avatar;
        [SerializeField] AvatarSpawner avatarSpawner;
        [SerializeField] MirrorAvatar mirrorAvatar;
        [SerializeField] ulong oculusID;
        [SerializeField] private PokeInteractable myAvatarButton;

        private void Start()
        {
            avatarSpawner.OnLocalAvatarLoaded += AvatarSpawner_OnLocalAvatarLoaded;
            avatarSpawner.OnEtitlementChecked += AvatarSpawner_OnEtitlementChecked;
            myAvatarButton.Disable();
        }

        private void AvatarSpawner_OnEtitlementChecked(Entitlement isEntitled)
        {
            if (isEntitled == Entitlement.Succeeded)
            {
                myAvatarButton.Enable();
            }
        }

        private void AvatarSpawner_OnLocalAvatarLoaded(FusionMetaAvatar _avatar)
        {
            avatar = _avatar;
        }

        [ContextMenu(nameof(LoadMyOculusAvatar))]
        public void LoadMyOculusAvatar()
        {
            //Number 0 represents "No preset avatar chosen"
            PlayerPrefs.SetInt("PresetAvatarIndex", 0);
            avatar.networkBehaviour.RPC_LoadMyOculusAvatar();
        }

        [ContextMenu(nameof(NextAvatar))]
        public void NextAvatar()
        {
            int newAvatar = 1;
            if (PlayerPrefs.GetInt("PresetAvatarIndex", 0) != 0)
            {
                newAvatar = PlayerPrefs.GetInt("PresetAvatarIndex");
            }
            newAvatar++;
            //There's 32 avatar presets, 1 to 32
            newAvatar %= 32;
            ChangeAvatar(newAvatar);
        }

        [ContextMenu(nameof(PreviousAvatar))]
        public void PreviousAvatar()
        {
            int newAvatar = 1;
            if (PlayerPrefs.GetInt("PresetAvatarIndex", 0) != 0)
            {
                newAvatar = PlayerPrefs.GetInt("PresetAvatarIndex");
            }
            newAvatar--;
            if (newAvatar < 1)
            {
                //There's 32 avatar presets, 1 to 32
                newAvatar = 32;
            }
            ChangeAvatar(newAvatar);
        }

        public void ChangeAvatar(int index)
        {
            PlayerPrefs.SetInt("PresetAvatarIndex", index);
            avatar.networkBehaviour.RPC_LoadLocalAvatarIndex(index);
        }
    }
}
