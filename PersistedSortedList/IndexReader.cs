using PersistedSortedList;

namespace SortedFileList
{
    public class IndexReader : IIndexReader
    {
        private IFileAdapter _fileAdapter;

        public IndexReader(string name)
        {
            _fileAdapter = new FileAdapter(name + ".index");
        }

        public IndexReader(
            string name,
            IFileAdapter fileAdapter)
        {
            _fileAdapter = fileAdapter;
        }

        public Node Create()
        {
            var node = new Node
            {
                Position = (int)_fileAdapter.Last
            };

            _fileAdapter.Write(_fileAdapter.Last, BTree.Serialize(node));

            return node;
        }

        public Node Get(int reference)
        {
            var buffer = _fileAdapter.Read(reference, BTree.NodeLength);
            var node = BTree.DeserializeNode(buffer);
            node.Position = reference;

            return node;
        }

        public void Update(Node node)
        {
            _fileAdapter.Write(node.Position, BTree.Serialize(node));
        }
    }
}