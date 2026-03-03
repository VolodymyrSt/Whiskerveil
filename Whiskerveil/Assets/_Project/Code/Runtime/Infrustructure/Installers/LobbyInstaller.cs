using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.LobbySlotServices;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.Installers
{
    public class LobbyInstaller : MonoInstaller
    {
        [SerializeField] private List<PlayerPlacementSlot> _playerPlacementSlots = new();

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LobbySlotService>().AsSingle().WithArguments(_playerPlacementSlots);
        }
    }
}