namespace PersistedSortedList
{
    public class IndexReader : IIndexReader
    {
        private readonly IFileAdapter _fileAdapter;

        public IndexReader(string name)
        {
            _fileAdapter = new FileAdapter(name + ".index");
        }

        public IndexReader(IFileAdapter fileAdapter)
        {
            _fileAdapter = fileAdapter;
        }

        public Node Create()
        {
            var node = new Node
            {
                Position = (int)_fileAdapter.Last
            };

            _fileAdapter.Write(_fileAdapter.Last, Node.Serialize(node));

            return node;
        }

        public Node Get(int reference)
        {
            var buffer = _fileAdapter.Read(reference, Node.NodeLength);
            var node = Node.DeserializeNode(buffer);
            node.Position = reference;

            return node;
        }

        public void Update(Node node)
        {
            _fileAdapter.Write(node.Position, Node.Serialize(node));
        }
    }
}