using Render;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Filtering;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace TeklaMedabilAPIs
{
    public class ModeloTekla
    {
        public void Set(string propriedade, string valor)
        {
            modelo.GetProjectInfo().SetUserProperty(propriedade, valor);
        }
        public string Get(string propriedade)
        {
            string valor = "";
            modelo.GetProjectInfo().GetUserProperty(propriedade, ref valor);
            return valor;
        }
        private Model _modelo { get; set; }
        public Model modelo
        {
            get
            {
                if (_modelo == null)
                {
                    _modelo = new Model();
                }
                return _modelo;
            }
        }
        public bool modeloAberto
        {
            get
            {
                return modelo.GetConnectionStatus();
            }
        }
        private List<Etapa> _etapas { get; set; }
        public List<Etapa> GetEtapas(bool reset = false)
        {
            if(_etapas == null| reset)
            {
                
                var etapas = modelo.GetPhases().OfType<Phase>().ToList();
                _etapas = etapas.Select(x => new Etapa(x, this)).ToList();
            }
            return _etapas;
        }
        private List<Marca> _marcas { get; set; }
        public List<Marca> GetMarcas(bool update = false)
        {
            Assembly.AssemblyTypeEnum tipo = Assembly.AssemblyTypeEnum.STEEL_ASSEMBLY;
            if (_marcas==null| update)
            {
                
                _marcas = new List<Marca>();
                ModelObjectEnumerator.AutoFetch = true;
                var pcs = modelo.GetModelObjectSelector().GetAllObjectsWithType(Tekla.Structures.Model.ModelObject.ModelObjectEnum.ASSEMBLY).Getlista<Assembly>();
                pcs = pcs.FindAll(x => x.GetAssemblyType() == tipo).ToList();
                _marcas = pcs.Select(x=> new Marca(x,this)).ToList();
                
            }
            return _marcas;
        }
        public void ResetVisuais()
        {
            ModelObjectVisualization.ClearAllTemporaryStates();
        }
        public void SetVisivel(List<Marca> itens, bool status = false)
        {
            ModelObjectVisualization.SetTransparency(itens.SelectMany(x=>x.GetPosicoes()).ToList().Cast<ModelObject>().ToList(), status? TemporaryTransparency.VISIBLE:TemporaryTransparency.HIDDEN);
        }
        public void SetCor(List<Marca> itens, Tekla.Structures.Model.UI.Color cor)
        {
            ModelObjectVisualization.SetTemporaryState(itens.SelectMany(x => x.GetPosicoes()).ToList().Cast<ModelObject>().ToList(), cor);
        }
        public List<Part> GetPosicoes()
        {
            ModelObjectEnumerator.AutoFetch = true;
            var tipos = new[] { typeof(Beam), typeof(BentPlate), typeof(ContourPlate), typeof(PolyBeam) };
            var itens = this.modelo.GetModelObjectSelector().GetAllObjectsWithType(tipos).Getlista<Part>();

            return itens;
        }
    }
}
