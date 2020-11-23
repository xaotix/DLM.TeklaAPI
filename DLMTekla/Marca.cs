using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;


namespace TeklaMedabilAPIs
{
    public class Marca
    {
        public string Get(string atributo)
        {
            string ret ="";
            var s = this._pecaTekla.GetUserProperty(atributo, ref ret);
            if(s)
            {
                return ret;
            }
            return "";
        }
        public void Set(string atributo, string valor)
        {
            this._pecaTekla.SetUserProperty(atributo, valor);
        }
        private Point _origem { get; set; }
        public Point GetOrigem()
        {
            if(_origem==null)
            {
                _origem = this._pecaTekla.GetCoordinateSystem().Origin;
            }
            return _origem;
        }
        public Etapa GetEtapa(bool reset = false)
        {
            if(this._etapa==null | reset)
            {
                Phase s;
                var p = _pecaTekla.GetPhase(out s);
                if (p)
                {
                    var sp = this.modelo.GetEtapas().Find(x => x.numero == s.PhaseNumber);
                    if (sp != null)
                    {
                        _etapa = sp;

                    }
                }
            }

            if(_etapa!=null)
            {
                return _etapa;
            }
            return new Etapa(new Phase(), this.modelo);
        }
        public List<Marca> marcas { get; set; } = new List<Marca>();
        public override string ToString()
        {
            return$"[{nome}]";
        }
        private Etapa _etapa { get; set; }
        private string _nome { get; set; }
        public string nome
        {
            get
            {
                if (_nome == null)
                {
                    string nome = "";
                    var ok = this._pecaTekla.GetReportProperty("ASSEMBLY_POS", ref nome);
                    _nome = nome;
                }
                return _nome;
            }
        }
        private double _peso { get; set; } = -1;
        public double peso
        {
            get
            {
                if(_peso ==-1)
                {
                    double peso = 0;
                    var ok = this._pecaTekla.GetReportProperty("WEIGHT_NET", ref peso);
                    _peso = Math.Round(peso,3);
                }
                return _peso;
            }
        }
        private int _quantidade { get; set; } = -1;
        public int quantidade
        {
            get
            {
                if(this.marcas.Count==0)
                {
                    if(_quantidade<0)
                    {
                        int qtd = 0;
                        _pecaTekla.GetReportProperty("NUMBER", ref qtd);
                        _quantidade = qtd;
                    }
                    return _quantidade;
                }
                return this.marcas.Count;
            }
        }

        private List<Part> _posicoes { get; set; }

        public List<Part> GetPosicoes(bool reset = false)
        {
            if(_posicoes==null|reset)
            {
                _posicoes = new List<Part>();
                List<Tekla.Structures.Model.Part> parts = new List<Tekla.Structures.Model.Part>();

                var principal = this._pecaTekla.GetMainPart();
                parts.Add(principal as Part);

                var arr = this._pecaTekla.GetSecondaries().ToArray().ToList();

                parts.AddRange(arr.Select(x => x as Part));

            }
            return _posicoes;
        }

        public ModeloTekla modelo { get; set; }
        private Assembly _pecaTekla { get; set; }
        public Marca(Assembly peca, ModeloTekla modelo)
        {
            this._pecaTekla = peca;
            this.modelo = modelo;
      
        }
        
        public Marca(List<Marca> marcas)
        {
            this.marcas = marcas;
            this.modelo = marcas[0].modelo;
            this._pecaTekla = marcas[0]._pecaTekla;
          
        }

    }
    public class Etapa
    {
        private List<Marca> _marcas { get; set; }
        public List<Marca> GetMarcas(bool reset = false)
        {
            if(_marcas == null| reset)
            {
                _marcas = new List<Marca>();
                foreach(var s in this.modelo.GetMarcas())
                {
                    if(s.GetEtapa().numero == this.numero)
                    {
                        _marcas.Add(s);
                    }
                }
            }
            var mms = _marcas.GroupBy(x => x.nome).Select(x => new Marca(x.ToList())).ToList();
             return mms;
        }
        public override string ToString()
        {
            return this.nome;
        }
        public ModeloTekla modelo { get; set; }
        public int numero { get; set; } = 0;
        public string nome { get; set; } = "";
        public Phase etapaTekla { get; set; }
        public Etapa(Phase etapa, ModeloTekla modelo)
        {
            this.etapaTekla = etapa;
            this.modelo = modelo;
            this.modelo = modelo;
            this.numero = etapa.PhaseNumber;
            this.nome = etapa.PhaseName;
        }
    }
}
