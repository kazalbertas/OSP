using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans;
using Orleans.Streams;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Operators
{
    public abstract class SourceSMSProvider<T> : Source<T>
    {

        public override async Task Start() 
        {
            //var primaryKey = this.GetPrimaryKey(out StreamName);
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<String>(GetStreamID(), GetStreamName());

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();

            if (subscriptionHandles.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles)
                {
                    await subscriptionHandle.ResumeAsync(OnNextMessage);
                }
            }

            await stream.SubscribeAsync(OnNextMessage);
        }

        private async Task OnNextMessage(string message, StreamSequenceToken sequenceToken)
        {
            
            T item = ProcessMessage(message);
            Data<T> dt = new Data<T>(GetKey(item), item);
            (var NextOperatorId, var NextOperatorClass) = _delegator.DelegateToProcess(dt.Key);
            var grain = GrainFactory.GetGrain<IOperator>(NextOperatorId, NextOperatorClass.FullName);
            grain.Process(dt, GetMetadata());
            // need to add watermarks
            //return Task.CompletedTask;
        }

        public abstract Guid GetStreamID();
        public abstract string GetStreamName();

    }
}
