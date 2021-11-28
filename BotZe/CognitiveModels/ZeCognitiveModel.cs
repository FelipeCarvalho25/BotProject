using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EmptyBotZe.CognitiveModels
{
    public class ZeCognitiveModel : IRecognizerConvert
    {
        public string Text;
        public string AlteredText;
        public enum Intent
        {
            AbrirChamado,
            JornadaMOPP,
            JornadaOcorrencias,
            JornadaRefeicoes,
            IndagaPrazo,
            Saudacao,
            None
        };
        public Dictionary<Intent, IntentScore> Intents;

        public class _Entities
        {
            // Built-in entities
            public string acao;
            public string chamado;
            public string incidente;
            public string Mopp;
            public string ocorrencia;
            public string prazo;
            public string problema;
            public string refeicao;
            public string saber;
            public string tempo;
            // Lists
            public string[][] Abertura;
            public string[][] Prazo;
            public string[][] Saber;


            // Instance
            public class _Instance
            {
                 public InstanceData[] acao;
                public InstanceData[] chamado;
                public InstanceData[] incidente;
                public InstanceData[] Mopp;
                public InstanceData[] ocorrencia;
                public InstanceData[] prazo;
                public InstanceData[] problema;
                public InstanceData[] refeicao;
                public InstanceData[] saber;
                public InstanceData[] tempo;
                public InstanceData[] Abertura;
                public InstanceData[] Prazo;
                public InstanceData[] Saber;
            }
            [JsonProperty("$instance")]
            public _Instance _instance;
        }
        public _Entities Entities;

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> Properties { get; set; }

        public void Convert(dynamic result)
        {
            var app = JsonConvert.DeserializeObject<ZeCognitiveModel>(JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) TopIntent()
        {
            Intent maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }
            return (maxIntent, max);
        }
    }
}
