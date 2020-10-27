using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;


namespace TeklaMedabilAPIs
{
    public class Marca
    {
        public string tipo { get; set; } = "";
        public override string ToString()
        {
            return$"[{tipo} - {nome}";
        }
        private Etapa _etapa { get; set; }
        public string nome { get; set; } = "";

        private List<Part> _pecas { get; set; }

        public List<Part> GetPecas(bool reset = false)
        {
            if(_pecas==null|reset)
            {
                _pecas = new List<Part>();
                _pecas.AddRange(this.modelo.GetPosicoes(this.pecaTekla));
            }
            return _pecas;
        }
        public Etapa GetEtapa(bool reset = false)
        {
            if(_etapa==null | reset)
            {
               _etapa = this.modelo.GetEtapa(this);
            }
            return _etapa;
        }

        public ModeloTekla modelo { get; set; }
        public Assembly pecaTekla { get; set; }
        public Marca(Assembly peca, ModeloTekla modelo)
        {
            this.pecaTekla = peca;
            this.modelo = modelo;
            this.nome = this.pecaTekla.AssemblyNumber.Prefix + "-" + this.pecaTekla.AssemblyNumber.StartNumber;
            //this.tipo = ((Part)peca.GetMainPart()).GetType().ToString();
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
            return _marcas;
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
