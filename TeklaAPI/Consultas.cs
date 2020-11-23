using Conexoes.Acessos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.DrawingInternal;
using Tekla.Structures.Model;
using TeklaMedabilAPIs;

namespace MedailTeklaAPI
{
    internal static class Consultas
    {
        internal static void ImportarSAP()
        {
            var ps = modelo.Get("pedido");
            PedidoSAP ped = null;

            if (ps != "")
            {
                ped = Consultas.GetPedidos().Find(x => x.pedido.ToUpper() == ps);
                if (ped != null)
                {
                    if (Conexoes.Utilz.Pergunta($"Deseja alterar o pedido?\nPedido encontrado:{ped.ToString()}"))
                    {
                        return;
                    }
                }
            }
            if(ped==null)
            {
            ped = Consultas.SelecionarPedido();
            }
            if (ped == null)
            {
                return;
            }else if (!Conexoes.Utilz.Pergunta($"Deseja continuar?\nTem certeza que selecionou o pedido correto? \nPedido Selecionado:{ped.ToString()}"))
            {
                return;
            }

            modelo.Set("pedido", ped.pedido);
            modelo.Set("importado_por", Conexoes.Vars.UsuarioAtual);
            modelo.Set("importado_por_data", DateTime.Now.ToString());

            var etps = ped.GetEtapas();
            Conexoes.Wait w = new Conexoes.Wait(etps.Count, 0, "Mapeando...");
            w.Show();
            foreach(var et in etps)
            {

                var pecaSAPs = et.GetpecasSAPAgrupado();
                w.somaProgresso();
                w.somaProgresso2(pecaSAPs.Count);
                foreach(var pc in pecaSAPs)
                {
                    var modelo = pc.GetMarcas();
                    foreach(var mpc in modelo)
                    {
                        mpc.Set("MEDABIL_PRODUZIDO", "");
                        mpc.Set("MEDABIL_EMBARCADO", "");

                        foreach (var s in pc.l.Celulas)
                        {
                            mpc.Set("MEDABIL_" + s.Coluna.ToUpper(), "");
                        }
                    }
                    for (int i = 0; i < pc.produzido; i++)
                    {
                        if(i<modelo.Count)
                        {
                            modelo[i].Set("MEDABIL_PRODUZIDO", "1");
                            foreach (var s in pc.l.Celulas)
                            {
                                modelo[i].Set("MEDABIL_" + s.Coluna.ToUpper(), s.Valor);
                            }
                        }
                       
                    }
                    for (int i = 0; i < pc.embarcado; i++)
                    {
                        if (i < modelo.Count)
                        {
                            modelo[i].Set("MEDABIL_EMBARCADO", "1");
                        }
                    }

                    w.somaProgresso2();
                }
                w.Zerar2();

            }
            w.Close();
        }
        internal static List<Marca> SelecionarEtapa()
        {
            var etapas = modelo.GetEtapas();
            var sel = Conexoes.Utilz.SelecionarObjeto(etapas, null, "Selecione");
            if (sel == null) { return new List<Marca>(); }
            var pcs = sel.GetMarcas();
            return pcs;
        }
        internal static PedidoSAP SelecionarPedido()
        {
            var lista = GetPedidos();
            var sel = Conexoes.Utilz.SelecionarObjeto(lista, null, "Selecione o pedido correspondente");
            if (sel == null) { return null; }
           ;
            return sel;
        }
        private static ModeloTekla _modelo { get; set; }

        internal static ModeloTekla modelo
        {
            get
            {
                if(_modelo==null)
                {
                    _modelo = new ModeloTekla();
                }
                return _modelo;
            }
        }
        internal static string banco_painel_de_obras2 { get; set; } = "painel_de_obras2";
        internal static string tabela_pecas { get; set; } = "pecas";
        internal static string banco_comum { get; set; } = "comum";
        internal static string tabela_pedidos_planejamento_copia { get; set; } = "pedidos_planejamento_copia";
        internal static string tabela_pep_planejamento_m_copia { get; set; } = "pep_planejamento_m_copia";
        internal static string servidor { get; set; } = "servidor";
        internal static string porta { get; set; } = "3306";
        internal static string user { get; set; } = "root";
        internal static string pass { get; set; } = "root";

        private static DB.Banco _db { get; set; }
        internal static DB.Banco db()
        {
           if(_db==null)
            {
                _db = new DB.Banco(servidor, porta, user, pass, banco_comum);
            }
            return _db;
        }


        private static List<PedidoSAP> _pedidos { get; set; } = new List<PedidoSAP>();

        internal static List<PedidoSAP> GetPedidos()
        {
            if(_pedidos.Count==0)
            {
                _pedidos = new List<PedidoSAP>();
                string comando = $"select * from  {banco_comum}.{tabela_pedidos_planejamento_copia}";
                var ls = db().Consulta(comando).Linhas;
                foreach (var l in ls)
                {
                    _pedidos.Add(new PedidoSAP(l));
                }
            }

            

            return _pedidos;
        }
        
    }
    internal class PedidoSAP
    {
        private List<EtapaSAP> _etapas { get; set; } = new List<EtapaSAP>();

        public List<EtapaSAP> GetEtapas()
        {
            if(_etapas.Count==0 && this.pedido.Length>5)
            {
                _etapas = new List<EtapaSAP>();
                string comando = $"select * from {Consultas.banco_comum}.{Consultas.tabela_pep_planejamento_m_copia} as pr where pr.pedido like '%{pedido}%'";
                var l = Consultas.db().Consulta(comando).Linhas;
                foreach(var s in l)
                {
                    _etapas.Add(new EtapaSAP(s, this));
                }
            }
            return _etapas;
        }
        public override string ToString()
        {
            return $"{this.pedido} - {this.nome}";
        }
        private DB.Linha l { get; set; } = new DB.Linha();
        private List<PecaSAP> _pecas { get; set; } = new List<PecaSAP>();

        public List<PecaSAP> GetPecas()
        {
            if(_pecas.Count==0 && pedido.Length>4)
            {
                _pecas = new List<PecaSAP>();
                var consulta = $"select * from {Consultas.banco_painel_de_obras2}.{Consultas.tabela_pecas} as pr where pr.pep like'%{this.pedido}%'";
                var l = Consultas.db().Consulta(consulta).Linhas;

                foreach(var s in l)
                {
                    _pecas.Add(new PecaSAP(s,this));
                }
            }
            else if(_pecas==null)
            {
                return new List<PecaSAP>();
            }
            return _pecas;
        }

        private string _pep { get; set; }
        public string pedido
        {
            get
            {
                if(_pep ==null)
                {
                    _pep = l.Get("pedido").ToString();
                }
                return _pep;
            }
        }

        private string _descricao { get; set; }
        public string nome
        {
            get
            {
                if(_descricao==null)
                {
                    _descricao = l.Get("nome").ToString();
                }
                return _descricao;
            }
        }

        public PedidoSAP(DB.Linha l)
        {
            this.l = l;
        }
    }
    internal class PecaSAP
    {
        public List<PecaSAP> pecas { get; set; } = new List<PecaSAP>();
        private string _material { get; set; }
        public string material
        {
            get
            {
                if (_material==null)
                {
                    if(pecas.Count>0)
                    {
                        _material = pecas[0].material;
                    }
                    _material = l.Get("material").ToString();
                }
                return _material;
            }
        }
        private int _qtd { get; set; } = -1;
        public int qtd
        {
            get
            {
                if (_qtd < 0)
                {
                    if(pecas.Count>0)
                    {
                        _qtd = pecas.Sum(x => x.qtd);
                    }
                    _qtd = l.Get("qtd").Int;
                }
                return _qtd;
            }
        }
        private int _produzido { get; set; } = -1;
        public int produzido
        {
            get
            {
                if(_produzido<0)
                {
                    if (pecas.Count > 0)
                    {
                        _produzido = pecas.Sum(x => x.produzido);
                    }
                    _produzido = l.Get("produzido").Int;
                }
                return _produzido;
            }
        }
        private int _embarcado { get; set; } = -1;
        public int embarcado
        {
            get
            {
                if (_embarcado < 0)
                {
                    if (pecas.Count > 0)
                    {
                        _embarcado = pecas.Sum(x => x.embarcado);
                    }
                    _embarcado = l.Get("embarcado").Int;
                }
                return _embarcado;
            }
        }
        private List<Marca> _marcas { get; set; } = new List<Marca>();
        public List<Marca> GetMarcas()
        {
            if(_marcas.Count ==0)
            {
                _marcas = new List<Marca>();
                var et = GetEtapaSAP();
                if(et!=null)
                {
                    var ets = et.GetEtapa();
                    if(ets!=null)
                    {
                        var mm = ets.GetMarcas().Find(x => x.nome.ToUpper() == this.nome.ToUpper());
                        if(mm!=null)
                        {
                            _marcas = mm.marcas;
                        }
                    }
                }
            }
            return _marcas;
        }
        public EtapaSAP GetEtapaSAP()
        {
            if(_etapa==null && this.pep.Length>6)
            {
                if(this.pecas.Count>0)
                {
                    _etapa = this.pecas[0].GetEtapaSAP();
                }
                _etapa = this.pedido.GetEtapas().Find(x => this.pep.ToUpper().Contains(x.pep));
            }
            return _etapa;
        }
        public void SetEtapa(EtapaSAP et)
        {
            this._etapa = et;
        }
        private EtapaSAP _etapa { get; set; }
        public PedidoSAP pedido { get; set; }
        public override string ToString()
        {
            return $"[{nome}] - [{pep}]";
        }
        private string _nome { get; set; }
        public string nome
        {
            get
            {
                if(_nome == null)
                {
                    if(pecas.Count>0)
                    {
                        _nome = pecas[0].nome;
                    }
                    _nome = l.Get("marca").ToString();
                }
                return _nome;
            }
        }
        public DB.Linha l { get; set; } = new DB.Linha();
        private string _pep { get; set; }
        public string pep
        {
            get
            {
                if(_pep ==null)
                {
                    if(pecas.Count>0)
                    {
                        _pep = pecas[0].pep;
                    }
                    _pep = l.Get("pep").ToString();
                }
                return _pep;
            }
        }
        public PecaSAP(DB.Linha l, PedidoSAP pedido)
        {
            this.l = l;
            this.pedido = pedido;
        }
        public PecaSAP(List<PecaSAP> pecaSAPs)
        {
            this.pecas = pecaSAPs;
            if(this.pecas.Count>0)
            {
                this.l = this.pecas[0].l;
                this.pedido = this.pecas[0].pedido;
            }
        }
    }

    internal class EtapaSAP
    {
        private Etapa _etapa { get; set; }
        public Etapa GetEtapa()
        {
            if(_etapa==null)
            {
                var s = Consultas.modelo.GetEtapas().Find(x => this.pep.Replace(".", "").Replace("-", "").Contains(x.nome));

                if(s!=null)
                {
                    _etapa = s;
                }
            }

            return _etapa;
        }
        public override string ToString()
        {
            return $"{this.pep}";
        }
        private List<PecaSAP> _pecas { get; set; } = new List<PecaSAP>();
        private List<PecaSAP> _pecas_agrupado { get; set; } = new List<PecaSAP>();
        public List<PecaSAP> GetpecasSAPAgrupado()
        {
           if(_pecas_agrupado.Count==0 && GetpecasSAP().Count>0)
            {
                var s = this.GetpecasSAP().GroupBy(x => x.nome + "/" + x.material).ToList();
                _pecas_agrupado = new List<PecaSAP>();
                foreach (var p in s)
                {
                    _pecas_agrupado.Add(new PecaSAP(p.ToList()));
                }
            }
            return _pecas_agrupado;

        }
        public List<PecaSAP> GetpecasSAP()
        {
            if(_pecas.Count==0 && this.pep.Length>0)
            {
                _pecas = this.pedido.GetPecas().FindAll(x => x.pep.ToUpper().StartsWith(this.pep.ToUpper()));
                foreach(var pc in _pecas)
                {
                    pc.SetEtapa(this);
                }
            }
            return _pecas;
        }


        public PedidoSAP pedido { get; set; }

        private string _pep { get; set; }
        public string pep
        {
            get
            {
                if(_pep==null)
                {
                    _pep = l.Get("pep").ToString();
                }
                return _pep;
            }
        }
        private DB.Linha l { get; set; } = new DB.Linha();

        public EtapaSAP(DB.Linha l, PedidoSAP pedido)
        {
            this.l = l;
            this.pedido = pedido;
        }

    }

}
