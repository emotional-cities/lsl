﻿using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Bonsai.Lsl
{
    [WorkflowElementCategory(ElementCategory.Sink)]
    public class SendObjectLsl : SingleArgumentExpressionBuilder
    {
        public string StreamName { get; set; }
        public string StreamType { get; set; }
        public int ChannelCount { get; set; }
        public string Uid { get; set; }

        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var streamName = Expression.Parameter(typeof(string), "streamName");
            var streamType = Expression.Parameter(typeof(string), "streamType");
            var channelCount = Expression.Parameter(typeof(int), "channelCount");
            var source = arguments.First(); // input source
            var parameterTypes = source.Type.GetGenericArguments(); // source types
            var inputParameter = Expression.Parameter(parameterTypes[0], "inputParameter");
            var builder = Expression.Constant(this);

            // Generates required outlets
            List<Expression> outletExpressions = new List<Expression>();
            var buildStream = ObjectStreamBuilder.OutletStream(streamName, streamType, channelCount, inputParameter, outletExpressions);

            //                     this     .Process        <parameterTypes>(source)
            return Expression.Call(builder, nameof(Process), parameterTypes, source);
        }

        IObservable<List<double>> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Never(new List<double> { 1 }); 
        }
    }
}
