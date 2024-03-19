using Fusion;

namespace Chiligames.MetaFusionTemplate
{
    public class GetAuthority : NetworkBehaviour
    {
        public void RequestAuthority()
        {
            Object.RequestStateAuthority();
        }
    }
}
