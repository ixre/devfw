namespace Ops.Regions
{
    public struct Province
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    }
    public struct City
    {
        public int ID { get; set; }
        public int Pid { get; set; }
        public string Zip { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    }

    public struct District
    {
        public int ID { get; set; }
        public int Cid { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    }
}