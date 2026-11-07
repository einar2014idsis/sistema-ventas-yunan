// Pruebas aisladas: no acceden a SQL Server ni alteran datos reales.
const fs = require('fs');
const vm = require('vm');
const assert = require('assert');
const source = fs.readFileSync(require('path').join(__dirname, '../wwwroot/js/dashboard-ventas.js'), 'utf8');

async function escenario(cantidad) {
    const nodes = new Map(), rendered = {};
    function node(id) {
        if (!nodes.has(id)) {
            const n = {id, value:'', hidden:false, dataset:{}, style:{}, innerHTML:'', textContent:'', listeners:{}, addEventListener(type,fn){this.listeners[type]=fn;}, querySelector(){return {hidden:true,textContent:''};}};
            n.parentElement = n;
            nodes.set(id,n);
        }
        return nodes.get(id);
    }
    const form=node('formFiltrosVenta'), cliente=node('cliente'), metodo=node('metodo');
    form.querySelector = s => s.includes('clienteId')?cliente:metodo;
    const root=node('analiticaVentas');root.dataset.url='/datos';root.dataset.excel='/excel';
    node('periodoAnalitica').value='all';node('agrupacion').value='day';node('modoCategoria').value='importe';node('modoPago').value='transacciones';node('modoDia').value='importe';
    const productos=[{id:1,nombre:'Prenda A',categoriaId:1,categoria:'Faldas',stock:2,critico:true},{id:2,nombre:'Prenda B',categoriaId:2,categoria:'Casacas',stock:10,critico:false},{id:3,nombre:'Sin ventas',categoriaId:1,categoria:'Faldas',stock:4,critico:false}];
    const ventas=Array.from({length:cantidad},(_,i)=>({id:i+1,fecha:'2026-10-17T10:00:00',clienteId:i%2+1,metodo:'Yape',estado:'Completada',total:90,detalles:[{productoId:1,nombre:'Prenda A',categoriaId:1,categoria:'Faldas',unidades:2,baseImporte:60},{productoId:2,nombre:'Prenda B',categoriaId:2,categoria:'Casacas',unidades:1,baseImporte:40}]}));
    class Chart {constructor(canvas,config){rendered[canvas.id]=config;} destroy(){}}
    const context={document:{getElementById:node},fetch:async()=>({ok:true,json:async()=>({ventas,productos,hoy:'2026-11-04'}),text:async()=>''}),Chart,FormData:class { *[Symbol.iterator](){} },URLSearchParams,structuredClone,Date,Map,Set,Number,String,Math,console};
    vm.runInNewContext(source,context);
    await new Promise(resolve=>setTimeout(resolve,20));
    assert.equal(node('dashboardError').hidden,true, `error en escenario ${cantidad}`);
    assert.match(node('kpisAnalitica').innerHTML,new RegExp(`Pedidos</div><div class="kpi-value">${cantidad}</div>`));
    assert.match(node('kpisAnalitica').innerHTML,new RegExp(`Productos vendidos</div><div class="kpi-value">${cantidad*3}</div>`));
    assert.match(node('inventarioAnalitica').innerHTML,/Productos con stock crítico/);
    if(cantidad){
        assert.equal(rendered.chartTop.data.datasets[0].data[0],cantidad*2);
        const sum=rendered.chartCategorias.data.datasets[0].data.reduce((a,b)=>a+b,0);
        assert.ok(Math.abs(sum-cantidad*90)<1e-6,'el prorrateo debe coincidir con el total final');
        assert.equal(rendered.chartPagosNuevo.options.indexAxis,'y');
    } else assert.equal(node('chartEvolucion').style.visibility,'hidden');
    node('periodoAnalitica').value='7';node('periodoAnalitica').listeners.change();
    assert.equal(node('chartEvolucion').style.visibility,'hidden','sin ventas recientes no debe mostrar línea plana');
    console.log(`Escenario ${cantidad}: correcto`);
}
(async()=>{for(const n of [0,2,20,100,500]) await escenario(n);})().catch(e=>{console.error(e);process.exitCode=1;});
