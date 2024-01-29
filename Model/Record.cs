using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCallApi.Model
{
    internal class Record
    {
        public Record(string id, string nome, string descricao, string preco, string status)
        {
            //PartitionKey = partitionKey;
            //RowKey = rowKey;
            Id = id;
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            Status = status;
        }

        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Preco { get; set; }
        public string Status { get; set; }
    }
}
