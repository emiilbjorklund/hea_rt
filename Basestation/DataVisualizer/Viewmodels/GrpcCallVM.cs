namespace DataVisualizer.Viewmodels
{
    public class GrpcCallVM : BaseVM
    {

        public GrpcCallVM(GrpcCallType type, string address, string targetId, CapabilityVM capability)
        {
            Address = address;
            TargetId = targetId;
            Capability = capability;
            Type = type;
        }

        public string DisplayName => TargetId;

        public string Address { get; }
        public string TargetId { get; }
        public CapabilityVM Capability { get; }
        public GrpcCallType Type { get; }
    }

    public enum GrpcCallType
    {
        EcgSubscription,
        PpgSubscription,
        HeartrateSubscription
    }
}