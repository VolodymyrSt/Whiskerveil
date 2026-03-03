using _Project.Code.Runtime.CommonServices.RolePickerService;
using Unity.Netcode;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LevelEntryPoint : NetworkBehaviour
    {
        private IRolePicker _rolePicker;
        
        [Inject]
        private void Construct(IRolePicker rolePicker)
        {
            _rolePicker = rolePicker;
        }
    }
}