namespace CapacitaGRDApi.Util
{
    public class Paginado
    {

        public class FilterDataTable
        {
            public List<Columns> columns { get; set; }
            public int draw { get; set; }
            public int length { get; set; }
            public List<Order> order { get; set; }
            public Search search { get; set; }
            public int start { get; set; }
        }

        public class Columns
        {
            public string data { get; set; }
            public string name { get; set; }
            public bool orderable { get; set; }
            public bool searchable { get; set; }

            public Search search { get; set; }
        }

        public class Search
        {
            public string value { get; set; }
            public bool regex { get; set; }
        }

        public class Order
        {
            public int column { get; set; }
            public string dir { get; set; }
        }

        public class Filter
        {
            public string Where { get; set; }
            public string Order { get; set; } 
            public string Limit { get; set; } = " ";

        }


    }
}


