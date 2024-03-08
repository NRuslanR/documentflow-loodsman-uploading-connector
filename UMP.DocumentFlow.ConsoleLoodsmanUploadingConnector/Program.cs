using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using UMP.DocumentFlow.LoodsmanUploadingConnector;

namespace UMP.DocumentFlow.ConsoleLoodsmanUploadingConnector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LoodsmanUploadingConnectorApp
                        .Builder
                        .WithConsoleLoodsmanDocumentUploadingConnectorInputSource(args)
                        .WithStandardLoodsmanDocumentUploadingConnectorBuilder<DataFlowLoodsmanDocumentUploadingConnector>()
                        .Build()
                        .Run();
            }
            catch (Exception exception)
            {
                var errorMessage = 
                    exception is AggregateException aggregateException ? 
                        string.Join(
                            Environment.NewLine, 
                            aggregateException.InnerExceptions.Select(e => e.Message)) : exception.Message;

                Console.WriteLine(errorMessage);
            }
 
            Console.ReadKey();
        }
    }

    abstract class BaseItem
    {
        private double volume;

        protected  BaseItem(double volume)
        {
            Volume = volume;
        }
        
        public double Volume
        {
            get => volume;
            private set
            {
                if (Numbers.IsNegativeOrZero(value))
                    throw new ArgumentException();

                volume = value;
            }
        }
    }

    class Item : BaseItem
    {
        public Item(double volume) : base(volume)
        {
        }
    }

    class Container : BaseItem
    {
        private ContainerType type;

        private readonly ReserveWeightDeltaAdjustingSpecification weightAdjustingSpecification;

        private ICollection<BaseItem> items;
        
        public Container(ContainerInfo info, ReserveWeightDeltaAdjustingSpecification weightAdjustingSpecification) : base(info.Volume)
        {
            Type = info.Type;

            this.weightAdjustingSpecification = weightAdjustingSpecification;
        }

        private Container(double volume) : base(volume)
        {
            items = new List<BaseItem>();
        }

        public void Add(Item item)
        {
            if (!weightAdjustingSpecification.IsWeightAdjustingAllowed(this, item.Volume))
                throw new ArgumentException();

            items.Add(item);

            Weight = Numbers.Sum(Weight, item.Volume);
        }

        public ContainerType Type
        {
            get => type;
            set
            {
                if (Objects.IsNull(value))
                    throw new ArgumentException();

                type = value;
            }
        }

        public double Weight { get; private set; }
    }

    interface IContainerWeightAdjustingSpecification
    {
        bool IsWeightAdjustingAllowed(Container container, double volume);
    }

    class ReserveWeightDeltaAdjustingSpecification : IContainerWeightAdjustingSpecification 
    {
        public double ReserveWeightDelta { get; }

        public ReserveWeightDeltaAdjustingSpecification(double reserveWeightDelta = 0)
        {
            ReserveWeightDelta = reserveWeightDelta;
        }

        public bool IsWeightAdjustingAllowed(Container container, double volume)
        {
            return container.Weight + volume <= container.Volume - ReserveWeightDelta;
        }
    }

    class ContainerInfo
    {
        public ContainerType Type { get; }

        public double Volume { get; }

        public ContainerInfo(ContainerType type, double volume)
        {
            Type = type;
            Volume = volume;
        }
    }

    class ContainerType
    {
        private string name;

        public ContainerType(string name)
        {
            Name = name;
        }

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException();

                name = value;
            }
        }
    }

    static class Objects
    {
        public static bool IsNotNull(object v)
        {
            return !IsNull(v);
        }

        public static bool IsNull(object v)
        {
            return v == null;
        }
    }

    static class Numbers
    {
        public static bool IsNegativeOrZero(double v) => IsNegative(v) || IsZero(v);

        public static bool IsNegative(double v) => v < 0;

        public static bool IsZero(double v) => v == 0;

        public static double Sum(double a, double b) => a + b;
    }
}
