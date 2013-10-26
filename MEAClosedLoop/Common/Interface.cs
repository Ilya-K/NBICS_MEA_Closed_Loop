using System;
using System.ServiceModel;
using Common;

namespace Common.Interface
{
    public interface IProcessor
    {
        [OperationContract(IsOneWay = true)]
        void processOneSpike(Spike spike);

        [OperationContract(IsOneWay = true)]
        void processSpikes(Spike[] spikes);
    }

    [ServiceContract(CallbackContract = typeof(IProcessor), SessionMode = SessionMode.Required)]
    public interface IGenerator
    {
        [OperationContract(IsOneWay = true)]
        void connect();

        [OperationContract(IsOneWay = true)]
        void requestOneSpike();

        [OperationContract(IsOneWay = true)]
        void requestSpikes(int _count);
    }
}
