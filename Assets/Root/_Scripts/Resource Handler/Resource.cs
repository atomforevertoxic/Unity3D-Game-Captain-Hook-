namespace Root
{
    public class Resource
    {
        public int PlasticCount { get; private set; }
        public int RubberCount { get; private set; }
        public int MetalCount { get; private set; }

        public Resource()
        {
            PlasticCount = 0;
            RubberCount = 0;
            MetalCount = 0;
        }

        public Resource(int plastic, int rubber, int metal)
        {
            PlasticCount = plastic;
            RubberCount = rubber;
            MetalCount = metal;
        }

        public static Resource Plastic(int count)
        {
            return new Resource(count, 0, 0);
        }

        public static Resource Rubber(int count)
        {
            return new Resource(0, count, 0);
        }

        public static Resource Metal(int count)
        {
            return new Resource(0, 0, count);
        }

        public Resource GetResources()
        {
            return (Resource)this.MemberwiseClone();
        }

        public static Resource Sum(Resource res1, Resource res2)
        {
            return new Resource(
                res1.PlasticCount + res2.PlasticCount,
                res1.RubberCount + res2.RubberCount,
                res1.MetalCount + res2.MetalCount
                );
        }

        public bool Subtract(Resource other)
        {
            return PlasticCount >= other.PlasticCount && RubberCount >= other.RubberCount && MetalCount >= other.MetalCount; 
        }

        public Resource Negative()
        {
            return new Resource(-PlasticCount, -RubberCount, -MetalCount);
        }

        public int Count()
        {
            return PlasticCount + RubberCount + MetalCount;
        }

        public override string ToString()
        {
            return "RESOURCE (plastic: " + PlasticCount + ", rubber: " + RubberCount + ", metal: " + MetalCount + ")";
        }
    }
}
